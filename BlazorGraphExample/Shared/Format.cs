using ByteSizeLib;

namespace BlazorGraphExample
{
    public static class Format
    {
        public static string FormatFileSize(long size)
        {
            var byteSize = ByteSize.FromBytes(size);
            if (size > 1_000_000)
                return byteSize.ToString("MB");
            else
                return byteSize.ToString("KB");
        }
    }
}
