// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.Eventing.Models;
using FileEntity = cCoder.Data.Models.DMS.File;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal class FileEventService(IFileEventBroker fileEventBroker, ICoreAuthInfo authInfo)
    : IFileEventService
{
    public async ValueTask RaiseFileAddEventAsync(LocalFile entity)
    {
        EventMessage<FileEntity> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFile(file: entity),
        };

        await fileEventBroker.RaiseFileAddEventAsync(message: message);
    }

    public async ValueTask RaiseFileUpdateEventAsync(LocalFile entity)
    {
        EventMessage<FileEntity> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFile(file: entity),
        };

        await fileEventBroker.RaiseFileUpdateEventAsync(message: message);
    }

    public async ValueTask RaiseFileDeleteEventAsync(LocalFile entity)
    {
        EventMessage<FileEntity> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFile(file: entity),
        };

        await fileEventBroker.RaiseFileDeleteEventAsync(message: message);
    }

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