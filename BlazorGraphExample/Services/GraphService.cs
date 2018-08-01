using BlazorGraphExample.Services.GraphAPI;
using Microsoft.AspNetCore.Blazor;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace BlazorGraphExample.Services
{
    public class GraphService : IGraphService
    {
        const string GraphEndpoint_Me = "https://graph.microsoft.com/v1.0/me";
        const string GraphEndpoint_Drives = "https://graph.microsoft.com/v1.0/drives";
        const string GraphEndpoint_DriveRoot = "https://graph.microsoft.com/v1.0/me/drive/root";
        const string GraphEndpoint_DriveRootChildren = GraphEndpoint_DriveRoot + "/children";

        const string SkipTokenParam = "$skipToken=";
        
        private readonly IAuthService _authService;
        private readonly HttpClient _http;

        // cache token for a few minutes
        private string _idToken;
        private DateTime _idTokenCreated;
        private const int MaxTokenAgeSeconds = 60 * 5;

        // keep a small LRU queue of drive items
        private LinkedList<LRUCacheEntry<List<DriveItem>>> _itemsCache;
        private const int CacheLimit = 5;
        private const int MaxCacheAgeSeconds = 60 * 5; // only allow items less than 5 minutes to return from cache

        public GraphService(IAuthService authService, HttpClient http)
        {
            _authService = authService;
            _http = http;
            _itemsCache = new LinkedList<LRUCacheEntry<List<DriveItem>>>();
            _idToken = null;
        }

        private async Task<(bool tokenSuccess, string token)> _TryGetTokenAsync()
        {
            bool tokenSuccess = false;
            string token = null;
            if(_idToken == null || (DateTime.Now - _idTokenCreated).TotalSeconds > MaxTokenAgeSeconds)
            {
                (tokenSuccess, token) = await _authService.TryGetTokenAsync();
                if(tokenSuccess)
                {
                    _idToken = token;
                    _idTokenCreated = DateTime.Now;
                }
            }
            else
            {
                token = _idToken;
                tokenSuccess = true;
            }

            return (tokenSuccess, token);
        }

        private bool _TryGetRequestUrlForFolder(string path, out string requestUrl)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
                requestUrl = GraphEndpoint_DriveRoot;
            else
                requestUrl = GraphEndpoint_DriveRoot + ":" + WebUtility.UrlEncode(path);
            return true;
        }

        private bool _TryGetRequestUrlForChildren(string path, out string requestUrl)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
                requestUrl = GraphEndpoint_DriveRootChildren;
            else
                requestUrl = GraphEndpoint_DriveRoot + ":" + WebUtility.UrlEncode(path) + ":/children";
            return true;
        }

        private bool _TryGetRequestUrlWithPaging(GetDriveItemsRequest request, out string requestUrl)
        {
            if (_TryGetRequestUrlForChildren(request?.Path, out requestUrl))
            {
                int pageSize = Math.Max(request.PageSize, GetDriveItemsRequest.MinimumPageSize);
                requestUrl += "?$top=" + pageSize;

                if (!string.IsNullOrEmpty(request.SkipToken))
                    requestUrl += "&$skipToken=" + WebUtility.UrlEncode(request.SkipToken);

                return true;
            }
            else
                requestUrl = null;
            return false;
        }

        private string _GetSkipToken(string nextLink)
        {
            string skipToken = null;
            if(!string.IsNullOrEmpty(nextLink))
            {
                int index = nextLink.IndexOf(SkipTokenParam, StringComparison.OrdinalIgnoreCase);
                if(index != -1)
                {
                    skipToken = nextLink.Substring(index + SkipTokenParam.Length);
                    index = skipToken.IndexOf('&');
                    if(index != -1) // there's another parameter to trim off...
                        skipToken = skipToken.Substring(0, index);
                }
            }
            return skipToken;
        }

        public async Task<GraphUser> GetMeAsync()
        {
            (bool tokenSuccess, string token) = await _TryGetTokenAsync();
            if (tokenSuccess)
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return await _http.GetJsonAsync<GraphUser>(GraphEndpoint_Me);
            }
            else
                return null;
        }

        public async Task<GetDriveItemsResponse> GetDriveItemsAsync(GetDriveItemsRequest request)
        {
            if (_TryGetRequestUrlWithPaging(request, out string requestUrl))
            {
                (bool tokenSuccess, string token) = await _TryGetTokenAsync();
                if (tokenSuccess)
                {
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    string responseStr = await _http.GetStringAsync(requestUrl);
                    var response = SimpleJson.SimpleJson.DeserializeObject<GraphResponse<DriveItem[]>>(responseStr, new SimpleJson.DataContractJsonSerializerStrategy());
                    return new GetDriveItemsResponse(response.Value, _GetSkipToken(response.NextLink));
                }
            }
            return null;
        }

        public async Task<int> GetChildItemsCountAsync(string path)
        {
            int count = 0;
            if (_TryGetRequestUrlForFolder(path, out string requestUrl))
            {
                // trim response to just the Folder facet
                requestUrl += "?$select=folder";

                (bool tokenSuccess, string token) = await _TryGetTokenAsync();
                if (tokenSuccess)
                {
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await _http.GetJsonAsync<DriveItem>(requestUrl);
                    count = response?.IsFolder == true ? response.Folder.ChildCount : 0;
                }
            }
            return count;
        }

        public async Task<List<DriveItem>> GetDriveItemsAtPathAsync(string path = "", Action<int> progressCallback = null, bool bypassCache = false)
        {
            Console.WriteLine("bypass cache: " + bypassCache);

            List<DriveItem> items = null;

            progressCallback?.Invoke(0);

            if (!bypassCache && _TryGetFromCache(path, out items))
                progressCallback?.Invoke(items.Count);
            else
            {
                (bool tokenSuccess, string token) = await _TryGetTokenAsync();
                if (tokenSuccess)
                {
                    items = new List<DriveItem>();
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    GraphResponse<DriveItem[]> response;

                    if (string.IsNullOrEmpty(path) || path == "/")
                    {
                        response = await _http.GetJsonAsync<GraphResponse<DriveItem[]>>(GraphEndpoint_DriveRootChildren);
                        items.AddRange(response.Value); // get root children
                        Console.WriteLine("API Count " + response.Count);
                        progressCallback?.Invoke(items.Count);
                    }
                    else
                    {
                        string getItemsUrl = GraphEndpoint_DriveRoot + ":" + path + ":/children?$top=100$count=true";
                        do
                        {
                            string responseStr = await _http.GetStringAsync(getItemsUrl);
                            response = SimpleJson.SimpleJson.DeserializeObject<GraphResponse<DriveItem[]>>(responseStr, new SimpleJson.DataContractJsonSerializerStrategy());
                            Console.WriteLine("API Count " + response.Count);
                            items.AddRange(response.Value);
                            getItemsUrl = response.NextLink;
                            progressCallback?.Invoke(items.Count);
                        } while (!string.IsNullOrEmpty(response.NextLink));
                    }

                    _AddToCache(path, items);
                }
            }
            return items;
        }

        private bool _TryGetFromCache(string path, out List<DriveItem> items)
        {
            var entry = _itemsCache.FirstOrDefault(i => i.Key == path);
            bool found = false;

            if (entry != null)
            {
                if ((DateTime.Now - entry.Created).TotalSeconds > MaxCacheAgeSeconds)
                {
                    _itemsCache.Remove(entry);
                    items = new List<DriveItem>(0);
                }
                else
                {
                    entry.LastUsed = DateTime.Now;
                    items = entry.Value;
                    found = true;
                }
            }
            else
                items = new List<DriveItem>(0);

            return found;
        }

        private void _AddToCache(string path, List<DriveItem> items)
        {
            if (!_TryGetFromCache(path, out List<DriveItem> getItems))
            {
                if (_itemsCache.Count > CacheLimit)
                {
                    var prune = _itemsCache.OrderBy(c => c.LastUsed).Last();
                    _itemsCache.Remove(prune);
                }
                _itemsCache.AddFirst(new LRUCacheEntry<List<DriveItem>>()
                {
                    Key = path,
                    Value = items,
                    LastUsed = DateTime.Now,
                    Created = DateTime.Now
                });
            }
        }

        private class LRUCacheEntry<T>
        {
            public string Key { get; set; }
            public DateTime LastUsed { get; set; }
            public DateTime Created { get; set; }
            public T Value { get; set; }
        }
    }
}
