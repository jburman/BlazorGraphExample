using BlazorGraphExample.Services.GraphAPI;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private IAuthService _authService;
        private HttpClient _http;

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

        public async Task<DriveItem[]> GetDriveRootItemsAsync(string driveId = null)
        {
            (bool tokenSuccess, string token) = await _TryGetTokenAsync();
            if (tokenSuccess)
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                GraphResponse<DriveItem[]> response;

                if(!string.IsNullOrEmpty(driveId))
                    response = await _http.GetJsonAsync<GraphResponse<DriveItem[]>>(GraphEndpoint_Drives + "/" + driveId + "/root/children");
                else
                    response = await _http.GetJsonAsync<GraphResponse<DriveItem[]>>(GraphEndpoint_DriveRootChildren);
                return response.Value;
            }
            else
                return null;
        }

        public async Task<List<DriveItem>> GetDriveItemsAtPathAsync(string path = "", Action<int> progressCallback = null, bool bypassCache = false)
        {
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
                        items.AddRange((await _http.GetJsonAsync<GraphResponse<DriveItem[]>>(GraphEndpoint_DriveRootChildren)).Value); // get root children
                        progressCallback?.Invoke(items.Count);
                    }
                    else
                    {
                        string getItemsUrl = GraphEndpoint_DriveRoot + ":" + path + ":/children?$top=100";
                        do
                        {
                            string responseStr = await _http.GetStringAsync(getItemsUrl);
                            response = SimpleJson.SimpleJson.DeserializeObject<GraphResponse<DriveItem[]>>(responseStr, new SimpleJson.DataContractJsonSerializerStrategy());
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

        public async Task<Drive[]> GetDrivesAsync()
        {
            (bool tokenSuccess, string token) = await _TryGetTokenAsync();
            if (tokenSuccess)
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetJsonAsync<GraphResponse<Drive[]>>(GraphEndpoint_Me + "/drives");
                return response.Value;
            }
            else
                return null;
        }

        public async Task<Drive> GetDriveAsync(string driveId)
        {
            (bool tokenSuccess, string token) = await _TryGetTokenAsync();
            if (tokenSuccess)
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetJsonAsync<GraphResponse<Drive>>(GraphEndpoint_Drives + "/" + driveId);
                return response.Value;
            }
            else
                return null;
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
            List<DriveItem> getItems;
            if (!_TryGetFromCache(path, out getItems))
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
