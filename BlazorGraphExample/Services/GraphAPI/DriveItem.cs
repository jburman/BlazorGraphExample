using System;

namespace BlazorGraphExample.Services.GraphAPI
{
    public class DriveItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset LastModifiedDateTime { get; set; }
        public string ETag { get; set; }
        public string WebUrl { get; set; }
        public FileFacet File { get; set; }
        public FolderFacet Folder { get; set; }
        public ItemReference ParentReference { get; set; }

        public bool IsFile => File != null;
        public bool IsFolder => Folder != null;
    }
}
