using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Services.Orchestrations;

namespace cCoder.DocumentManagement.Brokers;

public interface IDmsInstanceFactory
{
    IDms CreateDms();
}

public class DmsInstanceFactory(IDmsOrchestrationService dmsOrchestrationService)
    : IDmsInstanceFactory
{
    public IDms CreateDms() => new Dms(dmsOrchestrationService);
}
