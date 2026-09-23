// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using Newtonsoft.Json;


namespace cCoder.DocumentManagement.Brokers;

public interface IJsonBroker
{
    object ParseJson(string json);
    T ParseJson<T>(string json);
    string Serialize(object value);
}

internal sealed class JsonBroker : IJsonBroker, IUtilityBroker
{
    public object ParseJson(string json) =>
        JsonConvert.DeserializeObject(value: json);

    public T ParseJson<T>(string json) =>
        JsonConvert.DeserializeObject<T>(value: json);

    public string Serialize(object value) =>
        JsonConvert.SerializeObject(value: value);
}