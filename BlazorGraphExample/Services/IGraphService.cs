using BlazorGraphExample.Services.GraphAPI;
using System.Threading.Tasks;

namespace BlazorGraphExample.Services
{
    public interface IGraphService
    {
        Task<GraphUser> GetMeAsync();
        Task<GetDriveItemsResponse> GetDriveItemsAsync(GetDriveItemsRequest request);
        Task<int> GetChildItemsCountAsync(string path);
    }
}