using BlazorGraphExample.Services.GraphAPI;
using System.Collections.Generic;

namespace BlazorGraphExample.Services
{
    public class GetDriveItemsResponse
    {
        public GetDriveItemsResponse(DriveItem[] driveItems, string skipToken)
        {
            DriveItems = new List<DriveItem>(driveItems);
            SkipToken = skipToken;
        }

        public List<DriveItem> DriveItems { get; private set; }
        public string SkipToken { get; private set; }
    }
}
