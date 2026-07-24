// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers;
using cCoder.DocumentManagement.Brokers.Events;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using DataFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal partial class FolderEventService(IFolderEventBroker folderEventBroker, IAuthInfoBroker authInfoBroker)
    : IFolderEventService
{
    public ValueTask RaiseFolderAddEventAsync(Folder entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<DataFolder> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFolder(folder: entity),
            };


            await folderEventBroker.RaiseFolderAddEventAsync(message: message);

        });

    public ValueTask RaiseFolderUpdateEventAsync(Folder entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<DataFolder> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFolder(folder: entity),
            };


            await folderEventBroker.RaiseFolderUpdateEventAsync(message: message);

        });

    public ValueTask RaiseFolderDeleteEventAsync(Folder entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<DataFolder> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFolder(folder: entity),
            };


            await folderEventBroker.RaiseFolderDeleteEventAsync(message: message);

        });

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