namespace cCoder.DocumentManagement.Services.Processings;

public interface IWebDavProcessingService
{
    ValueTask<DmsProcessingResponse> ProcessAsync(DmsProcessingRequest request);
}






