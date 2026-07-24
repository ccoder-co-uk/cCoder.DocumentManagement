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
    public cCoder.Data.Models.DMS.File Get(Guid id)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [id]);
            return processingService.Get(id: id);

        });

    public IQueryable<cCoder.Data.Models.DMS.File> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return processingService.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<cCoder.Data.Models.DMS.File> AddFileAsync(cCoder.Data.Models.DMS.File entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            cCoder.Data.Models.DMS.File result = await processingService.AddFileAsync(entity: entity);

            await eventService.RaiseFileAddEventAsync(entity: result);

            return result;

        });

    public ValueTask<cCoder.Data.Models.DMS.File> UpdateFileAsync(cCoder.Data.Models.DMS.File entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            cCoder.Data.Models.DMS.File result = await processingService.UpdateFileAsync(entity: entity);

            await eventService.RaiseFileUpdateEventAsync(entity: result);

            return result;

        });

    public ValueTask DeleteAsync(Guid id)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [id]);
            cCoder.Data.Models.DMS.File entity = processingService.GetAll(ignoreFilters: true)
    .FirstOrDefault(predicate: file => file.Id == id);


            if (entity == null)
            {
                return;
            }


            await eventService.RaiseFileDeleteEventAsync(entity: entity);

            await processingService.DeleteAsync(id: id);

        });

    public ValueTask<IEnumerable<Result<cCoder.Data.Models.DMS.File>>> AddOrUpdateFile(IEnumerable<cCoder.Data.Models.DMS.File> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.AddOrUpdateFile(items: items);

        });

    public ValueTask DeleteAllFileAsync(IEnumerable<cCoder.Data.Models.DMS.File> items)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [items]);
            return processingService.DeleteAllFileAsync(items: items);

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

    public DMSResult GetAppPath(App app, cCoder.DocumentManagement.Models.Path path, int version = 0)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, version]);
            return processingService.GetAppPath(app: app, path: path, version: version);

        });

    public IEnumerable<cCoder.Data.Models.DMS.File> SearchApp(App app, string needle)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, needle]);
            return processingService.SearchApp(app: app, needle: needle);

        });

    public ValueTask SaveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path, Stream content = null)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, content]);
            return processingService.SaveAppPathAsync(app: app, path: path, content: content);

        });

    public ValueTask DropAppPathAsync(App app, cCoder.DocumentManagement.Models.Path path, int version = 0)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, path, version]);
            return processingService.DropAppPathAsync(app: app, path: path, version: version);

        });

    public ValueTask CopyAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            return processingService.CopyAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);

        });

    public ValueTask MoveAppPathAsync(App app, cCoder.DocumentManagement.Models.Path oldPath, cCoder.DocumentManagement.Models.Path newPath)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [app, oldPath, newPath]);
            return processingService.MoveAppPathAsync(app: app, oldPath: oldPath, newPath: newPath);

        });
}