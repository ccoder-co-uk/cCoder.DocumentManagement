// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldRejectStoredDuplicateWithoutCallingInsert()
    {
        // Given
        currentUser = ToLocalUser(user: TestUsers.WithPrivilege(privilege: "folderrole_create", appId: 1));

        authorizationBrokerMock.Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: currentUser);

        var accessRole = currentUser.Roles.First();
        var folder = CreateFolder(folderRoles: [new FolderRole { RoleId = accessRole.RoleId, Role = accessRole.Role }]);
        var role = new Role { Id = Guid.NewGuid(), AppId = 1, Name = "Editors", Privs = "folder_read" };
        var link = new FolderRole { FolderId = folder.Id, RoleId = role.Id };
        SetupFolderRoleContext(folderRole: link, context: CreateFolderRoleContext(folder: folder, role: role));

        folderRoleServiceMock.Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { link }.AsQueryable());

        // When
        Exception exception = await Record.ExceptionAsync(testCode: async () =>
            await folderRoleProcessingService.AddFolderRoleAsync(newFolderRole: link));

        // Then
        Assert.NotNull(@object: exception);
        Assert.Contains(expectedSubstring: "already", actualString: exception.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
        folderRoleServiceMock.Verify(expression: service => service.AddFolderRoleAsync(newFolderRole: It.IsAny<FolderRole>()), times: Times.Never);
    }
}