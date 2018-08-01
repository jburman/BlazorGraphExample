using System.Runtime.Serialization;

namespace BlazorGraphExample.Services.GraphAPI
{
    [DataContract]
    public class GraphResponse<T>
    {
        [DataMember(Name = "value")]
        public T Value { get; set; }
        [DataMember(Name = "@odata.nextLink")]
        public string NextLink { get; set; }
        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
