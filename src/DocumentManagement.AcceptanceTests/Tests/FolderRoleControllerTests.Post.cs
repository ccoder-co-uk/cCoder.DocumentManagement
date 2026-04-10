using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderRoleControllerTests
{
    [Fact]
    public async Task Post_CreatesFolderRole()
    {
        // Given
        SeededFolderRoleContext seededContext = await SeedDatabase(false, "app_admin", "folder_read", "folderrole_create", "folderrole_delete");
        FolderRole actualFolderRole;

        // When
        actualFolderRole = await CreateFolderRoleAsync(new
        {
            folderId = seededContext.FolderId,
            roleId = seededContext.RoleId,
        });

        // Then
        actualFolderRole.Should().NotBeNull();
        actualFolderRole.FolderId.Should().Be(seededContext.FolderId);
        actualFolderRole.RoleId.Should().Be(seededContext.RoleId);

        await Teardown(seededContext);
    }
}





