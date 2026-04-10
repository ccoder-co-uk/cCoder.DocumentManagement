using cCoder.DocumentManagement.Services.Processings;


namespace cCoder.DocumentManagement.Services.Orchestrations;

public interface IDmsHttpRequestOrchestrationService
{
    ValueTask<DmsProcessingResponse> ProcessRequestAsync(HttpContext context);
}







