using BlazorGraphExample.Services.GraphAPI;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
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

        public GraphService(IAuthService authService, HttpClient http)
        {
            _authService = authService;
            _http = http;
        }

        public async Task<GraphUser> GetMeAsync()
        {
            (bool tokenSuccess, string token) = await _authService.TryGetTokenAsync();
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
            (bool tokenSuccess, string token) = await _authService.TryGetTokenAsync();
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

        public async Task<List<DriveItem>> GetDriveItemsAtPathAsync(string path = "", Action<int> progressCallback = null)
        {
            (bool tokenSuccess, string token) = await _authService.TryGetTokenAsync();
            if (tokenSuccess)
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                GraphResponse<DriveItem[]> response;
                var items = new List<DriveItem>();
                progressCallback?.Invoke(0);

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
                
                return items;
            }
            else
                return null;
        }

        public async Task<Drive[]> GetDrivesAsync()
        {
            (bool tokenSuccess, string token) = await _authService.TryGetTokenAsync();
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
            (bool tokenSuccess, string token) = await _authService.TryGetTokenAsync();
            if (tokenSuccess)
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _http.GetJsonAsync<GraphResponse<Drive>>(GraphEndpoint_Drives + "/" + driveId);
                return response.Value;
            }
            else
                return null;
        }
    }
}
