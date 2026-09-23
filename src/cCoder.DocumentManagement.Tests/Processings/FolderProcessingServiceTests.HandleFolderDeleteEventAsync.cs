// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
    [Fact]
    public async Task HandleFolderDeleteEventAsync_WhenFolderIsNull_ThrowsValidationExceptionAsync()
    {
        // Given
        Folder folder = null;

        // When
        Func<Task> action = async () =>
            await folderProcessingService.HandleFolderDeleteEventAsync(folder: folder);

        // Then
        await action.Should()
            .ThrowAsync<DocumentManagementValidationException>()
            .WithInnerException(innerException: typeof(ArgumentNullException));

        fileMutationOperationsExposureMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDeleteTrackedFolderContentsWhenHandleFolderDeleteEventAsync()
    {
        // Given
        Folder folder = CreateRandomFolder();

        cCoder.Data.Models.DMS.File file = new()
        {
            Id = Guid.NewGuid(),
            Folder = folder,
            FolderId = folder.Id,
            Name = "file.txt",
            Path = $"{folder.Path}/file.txt",
        };

        FileContent content = new()
        {
            Id = Guid.NewGuid(),
            FileId = file.Id,
            Version = 1,
            RawData = [1],
            Size = "1B",
            CreatedBy = "test-user",
            CreatedOn = DateTimeOffset.UtcNow,
        };

        folderServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: new[] { folder }.AsQueryable());

        fileMutationOperationsExposureMock
            .Setup(expression: x => x.GetAllFiles(ignoreFilters: true))
            .Returns(value: Array.Empty<cCoder.Data.Models.DMS.File>()
                .AsQueryable());

        fileServiceMock
            .Setup(expression: x => x.GetIdsByFolderIds(folderIds: It.Is<Guid[]>(match: ids => ids.Single() == folder.Id), ignoreFilters: true))
            .Returns(value: [file.Id]);

        fileContentOperationsExposureMock
            .Setup(expression: x => x.DeleteAllForFilesAsync(fileIds: It.Is<Guid[]>(match: ids => ids.Single() == file.Id)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await folderProcessingService.HandleFolderDeleteEventAsync(folder: folder);

        // Then
        folderServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Exactly(callCount: 3));

        fileServiceMock.Verify(
            expression: x => x.GetIdsByFolderIds(folderIds: It.Is<Guid[]>(match: ids => ids.Single() == folder.Id), ignoreFilters: true),
            times: Times.Once
        );

        fileContentOperationsExposureMock.Verify(
            expression: x => x.DeleteAllForFilesAsync(fileIds: It.Is<Guid[]>(match: ids => ids.Single() == file.Id)),
            times: Times.Once
        );

        fileMutationOperationsExposureMock.Verify(
            expression: x => x.GetAllFiles(ignoreFilters: true),
            times: Times.Once);

        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentOperationsExposureMock.VerifyNoOtherCalls();
        fileMutationOperationsExposureMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

}