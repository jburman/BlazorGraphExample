using System;

namespace BlazorGraphExample.Services
{
    public class GetDriveItemsRequest
    {
        public const int MinimumPageSize = 10;

        public GetDriveItemsRequest(string path, int pageSize = MinimumPageSize, string skipToken = null, bool allowCache = true)
        {
            Path = path;
            PageSize = Math.Min(pageSize, pageSize);
            SkipToken = skipToken;
            AllowCache = allowCache;
        }

        public string Path { get; private set; }
        public int PageSize { get; private set; }
        public string SkipToken { get; private set; }
        public bool AllowCache { get; private set; }
    }
}
