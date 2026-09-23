// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Dependencies;
using System.Net;
using System.Text;

namespace cCoder.DocumentManagement.Brokers;

internal sealed class StreamBroker : IStreamBroker, IUtilityBroker
{
    public Stream Create(byte[] content) =>
        new DocumentStreamDependency(buffer: content);

    public byte[] ReadAllBytes(Stream source)
    {
        using DocumentStreamDependency destination = new();
        source.CopyTo(destination: destination);
        return destination.ToArray();
    }

    public async ValueTask<byte[]> ReadAllBytesAsync(Stream source)
    {
        using DocumentStreamDependency destination = new();
        await source.CopyToAsync(destination: destination);
        return destination.ToArray();
    }

    public string DecodeUtf8(byte[] content) =>
        Encoding.UTF8.GetString(bytes: content);

    public byte[] EncodeUtf8(string content) =>
        Encoding.UTF8.GetBytes(s: content);

    public string DecodeUrl(string value) =>
        WebUtility.UrlDecode(encodedValue: value);
}