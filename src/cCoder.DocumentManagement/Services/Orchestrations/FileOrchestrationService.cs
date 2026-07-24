// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;

namespace cCoder.DocumentManagement.Services.Orchestrations;

internal partial class FileOrchestrationService(IFileProcessingService processingService, IFileEventProcessingService eventService) : IFileOrchestrationService
{
    public cCoder.Data.Models.DMS.File Get(Guid fileId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileId]);
            return processingService.Get(fileId: fileId);

        });

    public IQueryable<cCoder.Data.Models.DMS.File> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<cCoder.Data.Models.DMS.File> AddFileAsync(cCoder.Data.Models.DMS.File newFile)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [newFile]);
            cCoder.Data.Models.DMS.File result = await processingService.AddFileAsync(newFile: newFile);

            await eventService.RaiseFileAddEventAsync(entity: result);

            return result;

        });

    public ValueTask<cCoder.Data.Models.DMS.File> UpdateFileAsync(cCoder.Data.Models.DMS.File updatedFile)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [updatedFile]);
            cCoder.Data.Models.DMS.File result = await processingService.UpdateFileAsync(updatedFile: updatedFile);

            await eventService.RaiseFileUpdateEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteAsync(Guid fileId)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [fileId]);

            cCoder.Data.Models.DMS.File entity = processingService.GetAll(ignoreFilters: true)
    .FirstOrDefault(predicate: file => file.Id == fileId);


            if (entity == null)
            {
                return;
            }


            await eventService.RaiseFileDeleteEventAsync(entity: entity);

            await processingService.DeleteAsync(fileId: fileId);

        });

    public ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdateFile(IEnumerable<cCoder.Data.Models.DMS.File> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateFile(items: items);

        });

    public ValueTask DeleteAllFileAsync(IEnumerable<cCoder.Data.Models.DMS.File> deletedFile)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [deletedFile]);
            return processingService.DeleteAllFileAsync(deletedFile: deletedFile);

        });

    public cCoder.Data.Models.DMS.File GetByPath(int appId, string path)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path]);
            return processingService.GetByPath(appId: appId, path: path);

        });

    public ValueTask HandleFileDeleteEventAsync(cCoder.Data.Models.DMS.File file)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [file]);
            return processingService.HandleFileDeleteEventAsync(file: file);

        });

    public DMSResult GetAppPath(int appId, cCoder.DocumentManagement.Dependencies.Path path, int version = 0)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, version]);
            return processingService.GetAppPath(appId: appId, path: path, version: version);

        });

    public IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(int appId, string needle)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, needle]);
            return processingService.SearchApp(appId: appId, needle: needle);

        });

    public ValueTask SaveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, Stream content = null)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, content]);
            return processingService.SaveAppPathAsync(appId: appId, path: path, content: content);

        });

    public ValueTask DropAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path path, int version = 0)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, path, version]);
            return processingService.DropAppPathAsync(appId: appId, path: path, version: version);

        });

    public ValueTask CopyAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);
            return processingService.CopyAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

        });

    public ValueTask MoveAppPathAsync(int appId, cCoder.DocumentManagement.Dependencies.Path oldPath, cCoder.DocumentManagement.Dependencies.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [appId, oldPath, newPath]);
            return processingService.MoveAppPathAsync(appId: appId, oldPath: oldPath, newPath: newPath);

        });
}