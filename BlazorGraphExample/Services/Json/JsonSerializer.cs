using W8lessLabs.GraphAPI;

namespace BlazorGraphExample.Services
{
    public class JsonSerializer : IJsonSerializer
    {
        static SimpleJson.IJsonSerializerStrategy _Strategy = new SimpleJson.DataContractJsonSerializerStrategy();

        public T Deserialize<T>(string value) => SimpleJson.SimpleJson.DeserializeObject<T>(value, _Strategy);
        public string Serialize<T>(T value) => SimpleJson.SimpleJson.SerializeObject(value, _Strategy);
    }
}
