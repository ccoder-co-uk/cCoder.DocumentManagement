// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.Eventing.Models;
using FileEntity = cCoder.Data.Models.DMS.File;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal partial class FileEventService(IFileEventBroker fileEventBroker, IAuthInfoBroker authInfoBroker)
    : IFileEventService
{
    public ValueTask RaiseFileAddEventAsync(LocalFile entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            EventMessage<FileEntity> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFile(file: entity),
            };


            await fileEventBroker.RaiseFileAddEventAsync(message: message);

        });

    public ValueTask RaiseFileUpdateEventAsync(LocalFile entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            EventMessage<FileEntity> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFile(file: entity),
            };


            await fileEventBroker.RaiseFileUpdateEventAsync(message: message);

        });

    public ValueTask RaiseFileDeleteEventAsync(LocalFile entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            EventMessage<FileEntity> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFile(file: entity),
            };


            await fileEventBroker.RaiseFileDeleteEventAsync(message: message);

        });

    private static FileEntity ToExternalFile(LocalFile file) =>
        file == null
            ? null
            : new FileEntity
            {
                Id = file.Id,
                FolderId = file.FolderId,
                Name = file.Name,
                Description = file.Description,
                Path = file.Path,
                MimeType = file.MimeType,
                CreatedBy = file.CreatedBy,
                Size = file.Size,
                CreatedOn = file.CreatedOn,
                DeletedOn = file.DeletedOn,
            };
}