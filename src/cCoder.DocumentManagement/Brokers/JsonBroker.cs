// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public object ParseJson(string json) =>
        JsonConvert.DeserializeObject(value: json);

    public T ParseJson<T>(string json) =>
        JsonConvert.DeserializeObject<T>(value: json);

    public string Serialize(object value) =>
        JsonConvert.SerializeObject(value: value);
}