// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations.Events;


namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FileContentEventProcessingService(IFileContentEventService eventService) : IFileContentEventProcessingService
{
    public ValueTask RaiseFileContentAddEventAsync(FileContent fileContent)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileContent]);
            return eventService.RaiseFileContentAddEventAsync(fileContent: fileContent);
        });

    public ValueTask RaiseFileContentUpdateEventAsync(FileContent fileContent)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileContent]);
            return eventService.RaiseFileContentUpdateEventAsync(fileContent: fileContent);
        });

    public ValueTask RaiseFileContentDeleteEventAsync(FileContent fileContent)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileContent]);
            return eventService.RaiseFileContentDeleteEventAsync(fileContent: fileContent);
        });
}