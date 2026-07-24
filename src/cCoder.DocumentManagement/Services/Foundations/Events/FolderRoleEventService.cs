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
using DataFolderRole = cCoder.Data.Models.Security.FolderRole;


namespace cCoder.DocumentManagement.Services.Foundations.Events;

internal partial class FolderRoleEventService(
    IFolderRoleEventBroker folderRoleEventBroker,
    IAuthInfoBroker authInfoBroker
) : IFolderRoleEventService
{
    public ValueTask RaiseFolderRoleAddEventAsync(FolderRole entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            EventMessage<DataFolderRole> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFolderRole(folderRole: entity),
            };


            await folderRoleEventBroker.RaiseFolderRoleAddEventAsync(message: message);

        });

    public ValueTask RaiseFolderRoleDeleteEventAsync(FolderRole entity)
=>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);
            EventMessage<DataFolderRole> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetCurrentSsoUserId() },
                Data = ToExternalFolderRole(folderRole: entity),
            };


            await folderRoleEventBroker.RaiseFolderRoleDeleteEventAsync(message: message);

        });

    private static DataFolderRole ToExternalFolderRole(FolderRole folderRole) =>
        folderRole == null
            ? null
            : new DataFolderRole
            {
                FolderId = folderRole.FolderId,
                RoleId = folderRole.RoleId,
            };
}