// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using DataFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal class FolderEventService(IFolderEventBroker folderEventBroker, ICoreAuthInfo authInfo)
    : IFolderEventService
{
    public async ValueTask RaiseFolderAddEventAsync(Folder entity)
    {
        EventMessage<DataFolder> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFolder(folder: entity),
        };

        await folderEventBroker.RaiseFolderAddEventAsync(message: message);
    }

    public async ValueTask RaiseFolderUpdateEventAsync(Folder entity)
    {
        EventMessage<DataFolder> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFolder(folder: entity),
        };

        await folderEventBroker.RaiseFolderUpdateEventAsync(message: message);
    }

    public async ValueTask RaiseFolderDeleteEventAsync(Folder entity)
    {
        EventMessage<DataFolder> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFolder(folder: entity),
        };

        await folderEventBroker.RaiseFolderDeleteEventAsync(message: message);
    }

    private static DataFolder ToExternalFolder(Folder folder) =>
        folder == null
            ? null
            : new DataFolder
            {
                Id = folder.Id,
                AppId = folder.AppId,
                ParentId = folder.ParentId,
                Name = folder.Name,
                Path = folder.Path,
                DeletedOn = folder.DeletedOn,
            };
}