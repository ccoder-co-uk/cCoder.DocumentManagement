// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Services.Orchestrations;

namespace cCoder.DocumentManagement.Brokers;

public interface IDmsInstanceFactory
{
    IDms CreateDms();
}

internal sealed class DmsInstanceFactory(IDmsOrchestrationService dmsOrchestrationService)
    : IDmsInstanceFactory
{
    public IDms CreateDms() =>
        new Dms(dmsOrchestrationService: dmsOrchestrationService);
}