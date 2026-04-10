using Newtonsoft.Json;


namespace cCoder.DocumentManagement.Brokers;

public interface IJsonBroker
{
    object ParseJson(string json);
    T ParseJson<T>(string json);
    string Serialize(object value);
}

public class JsonBroker : IJsonBroker
{
    public object ParseJson(string json) => JsonConvert.DeserializeObject(json);

    public T ParseJson<T>(string json) => JsonConvert.DeserializeObject<T>(json);

    public string Serialize(object value) => JsonConvert.SerializeObject(value);
}

