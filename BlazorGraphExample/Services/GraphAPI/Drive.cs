namespace BlazorGraphExample.Services.GraphAPI
{
    public class Drive
    {
        public string Id { get; set; }
        public string DriveType { get; set; }
        public DriveOwner Owner { get; set; }
        public DriveQuota Quota { get; set; }
    }
}
