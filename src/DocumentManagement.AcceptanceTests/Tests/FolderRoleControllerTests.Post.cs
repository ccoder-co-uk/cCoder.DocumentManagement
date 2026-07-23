// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        SeededFolderRoleContext seededContext = await SeedDatabase(includeFolderRole: false, "app_admin", "folder_read", "folderrole_create", "folderrole_delete");
        FolderRole actualFolderRole;

        // When
        actualFolderRole = await CreateFolderRoleAsync(payload: new
        {
            folderId = seededContext.FolderId,
            roleId = seededContext.RoleId,
        });

        // Then
        actualFolderRole.Should()
            .NotBeNull();

        actualFolderRole.FolderId.Should()
            .Be(expected: seededContext.FolderId);

        actualFolderRole.RoleId.Should()
            .Be(expected: seededContext.RoleId);

        await Teardown(seededContext: seededContext);
    }
}