// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Brokers;

internal interface IStreamBroker
{
    Stream Create(byte[] content);

    byte[] ReadAllBytes(Stream source);

    ValueTask<byte[]> ReadAllBytesAsync(Stream source);

    string DecodeUtf8(byte[] content);

    byte[] EncodeUtf8(string content);

    string DecodeUrl(string value);
}