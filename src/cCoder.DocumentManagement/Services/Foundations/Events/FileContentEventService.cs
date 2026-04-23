using cCoder.Data;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using DataFile = cCoder.Data.Models.DMS.File;
using DataFileContent = cCoder.Data.Models.DMS.FileContent;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal class FileContentEventService(
    IFileContentEventBroker fileContentEventBroker,
    ICoreAuthInfo authInfo
) : IFileContentEventService
{
    public async ValueTask RaiseFileContentAddEventAsync(FileContent entity)
    {
        EventMessage<DataFileContent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFileContent(entity),
        };

        await fileContentEventBroker.RaiseFileContentAddEventAsync(message);
    }

    public async ValueTask RaiseFileContentUpdateEventAsync(FileContent entity)
    {
        EventMessage<DataFileContent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFileContent(entity),
        };

        await fileContentEventBroker.RaiseFileContentUpdateEventAsync(message);
    }

    public async ValueTask RaiseFileContentDeleteEventAsync(FileContent entity)
    {
        EventMessage<DataFileContent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFileContent(entity),
        };

        await fileContentEventBroker.RaiseFileContentDeleteEventAsync(message);
    }

    private static DataFileContent ToExternalFileContent(FileContent fileContent) =>
        fileContent == null
            ? null
            : new DataFileContent
            {
                Id = fileContent.Id,
                FileId = fileContent.FileId,
                Description = fileContent.Description,
                Size = fileContent.Size,
                CreatedBy = fileContent.CreatedBy,
                CreatedOn = fileContent.CreatedOn,
                Version = fileContent.Version,
                RawData = fileContent.RawData,
                File = ToExternalFile(fileContent.File),
            };

    private static DataFile ToExternalFile(LocalFile file) =>
        file == null
            ? null
            : new DataFile
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









