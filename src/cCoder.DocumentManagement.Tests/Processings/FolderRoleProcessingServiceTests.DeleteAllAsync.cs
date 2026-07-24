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
        var exception = await act.Should()
            .ThrowAsync<DocumentManagementServiceException>();

        exception.Which.InnerException.Should()
            .BeOfType<DocumentManagementServiceException>()
            .Which.InnerException.Should()
            .BeOfType<System.Security.SecurityException>();
    }

}