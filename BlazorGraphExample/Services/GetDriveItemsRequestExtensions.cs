namespace BlazorGraphExample.Services
{
    public static class GetDriveItemsRequestExtensions
    {
        public static string GetCacheKey(this GetDriveItemsRequest request) => request?.Path + "_" + request?.SkipToken;
    }
}
