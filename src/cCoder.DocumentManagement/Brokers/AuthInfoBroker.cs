// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.Data;

namespace cCoder.DocumentManagement.Brokers;

public interface IAuthInfoBroker
{
    string GetCurrentSsoUserId();
}

internal sealed class AuthInfoBroker(ICoreAuthInfo authInfo)
    : IAuthInfoBroker, IUtilityBroker
{
    public string GetCurrentSsoUserId() =>
        authInfo.SSOUserId;
}