namespace cCoder.DocumentManagement.Services.Processings;

public interface IDmsProcessingService
{
    ValueTask<DmsProcessingResponse> ProcessAsync(DmsProcessingRequest request);
}






