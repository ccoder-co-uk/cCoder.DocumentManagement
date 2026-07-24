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
    public FileContent Get(Guid id)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [id]);
            return service.Get(id: id);

        });

    public IQueryable<FileContent> GetAll(bool ignoreFilters = false)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [ignoreFilters]);
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<FileContent> AddAsync(FileContent entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return service.AddAsync(fileContent: entity);

        });

    public ValueTask<FileContent> UpdateAsync(FileContent entity)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [entity]);
            return service.UpdateAsync(fileContent: entity);

        });

    public ValueTask DeleteAsync(Guid id)
=>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [id]);
            return service.DeleteAsync(id: id);

        });

    public ValueTask<IEnumerable<Result<FileContent>>> AddOrUpdate(IEnumerable<FileContent> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            List<Result<FileContent>> results = new List<Result<FileContent>>();


            foreach (FileContent item in items)
            {
                try
                {
                    FileContent savedItem = item.Id == Guid.Empty ? await AddAsync(entity: item) : await UpdateAsync(entity: item);

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

    public ValueTask DeleteAllAsync(IEnumerable<FileContent> items)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [items]);
            foreach (FileContent item in items)
            {
                await DeleteAsync(id: item.Id);
            }

        });
}