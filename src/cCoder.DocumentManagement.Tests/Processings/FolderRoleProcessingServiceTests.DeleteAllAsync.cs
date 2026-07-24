// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenItemUsesCompositeKeyForDeleteAllAsync()
    {
        // Given
        FolderRole link = new() { FolderId = Guid.NewGuid(), RoleId = Guid.NewGuid() };

        // When
        Func<Task> act = async () =>
            await folderRoleProcessingService.DeleteAllFolderRoleAsync(deletedFolderRole: new[] { link });

        // Then
        await act.Should()
            .ThrowAsync<System.Security.SecurityException>();
    }

}