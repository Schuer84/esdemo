using Newtonsoft.Json;

namespace SqlStreamStore.Demo.Serializers.Json
{
    public class DefaultJsonSerializer : IJsonSerializer
    {
        public object Deserialize(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}