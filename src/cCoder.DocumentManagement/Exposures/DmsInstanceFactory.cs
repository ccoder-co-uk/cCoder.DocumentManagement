// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;

namespace cCoder.DocumentManagement.Exposures;

public interface IDmsInstanceFactory
{
    Dms CreateDms();
}

internal sealed class DmsInstanceFactory(IDmsOrchestrationService dmsOrchestrationService)
    : IDmsInstanceFactory
{
    public Dms CreateDms() =>
        new Dms(dmsOrchestrationService: dmsOrchestrationService);
}