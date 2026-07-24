// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Orchestrations;

namespace cCoder.DocumentManagement.Exposures;

public interface IDmsInstanceFactory
{
    IDms CreateDmsInstance();
}

internal sealed class DmsInstanceFactory(IDmsOrchestrationService dmsOrchestrationService)
    : IDmsInstanceFactory
{
    public IDms CreateDmsInstance() =>
        new Dms(dmsOrchestrationService: dmsOrchestrationService);
}