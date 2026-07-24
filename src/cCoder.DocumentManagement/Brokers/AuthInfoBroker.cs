// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;

namespace cCoder.DocumentManagement.Brokers;

public interface IAuthInfoBroker
{
    string GetCurrentSsoUserId();
}

internal sealed class AuthInfoBroker(ICoreAuthInfo authInfo) : IAuthInfoBroker
{
    public string GetCurrentSsoUserId() =>
        authInfo.SSOUserId;
}