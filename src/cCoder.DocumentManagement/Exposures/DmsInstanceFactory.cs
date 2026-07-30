// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;

namespace cCoder.DocumentManagement.Exposures;

public interface IDmsInstanceFactory
{
    IDms CreateDms();
}

internal sealed class DmsInstanceFactory(
    IDmsOrchestrationService dmsOrchestrationService)
    : IDmsInstanceFactory
{
    public IDms CreateDms() =>
        new Dms(
            dmsOrchestrationService: dmsOrchestrationService);
}