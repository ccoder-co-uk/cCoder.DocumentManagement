// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Foundations;

namespace cCoder.DocumentManagement.Services.Processings;

internal partial class FileContentProcessingService(IFileContentService service) : IFileContentProcessingService
{
    public FileContent Get(Guid fileContentId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileContentId]);
            return service.Get(fileContentId: fileContentId);

        });

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<FileContent> AddFileContentAsync(FileContent newFileContent)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [newFileContent]);
            return service.AddFileContentAsync(newFileContent: newFileContent);

        });

    public ValueTask<FileContent> UpdateFileContentAsync(FileContent updatedFileContent)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [updatedFileContent]);
            return service.UpdateFileContentAsync(updatedFileContent: updatedFileContent);

        });

    public ValueTask DeleteAsync(Guid fileContentId)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [fileContentId]);
            return service.DeleteAsync(fileContentId: fileContentId);

        });

    public ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdateFileContent(IEnumerable<FileContent> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            List<Result<FileContent>> results = new List<Result<FileContent>>();


            foreach (FileContent item in items)
            {
                try
                {
                    FileContent savedItem = item.Id == Guid.Empty ? await AddFileContentAsync(newFileContent: item) : await UpdateFileContentAsync(updatedFileContent: item);

                    results.Add(item: new Result<FileContent>
                    {
                        Success = true,
                        Item = savedItem,
                        Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(item: new Result<FileContent>
                    {
                        Success = false,
                        Item = item,
                        Message = ex.Message
                    });
                }
            }


            return (IEnumerable<Result<FileContent>>)results;

        });

    public ValueTask DeleteAllFileContentAsync(IEnumerable<FileContent> deletedFileContent)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [deletedFileContent]);
            foreach (FileContent item in deletedFileContent)
            {
                await DeleteAsync(fileContentId: item.Id);
            }

        });
}