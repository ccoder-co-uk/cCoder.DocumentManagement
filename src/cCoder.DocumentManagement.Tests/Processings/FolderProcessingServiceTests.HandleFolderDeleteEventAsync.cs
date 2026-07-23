// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FolderProcessingServiceTests
{
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
        folderServiceMock.Setup(expression: x => x.GetAll(true)).Returns(value: new[] { folder }.AsQueryable());
        fileServiceMock
            .Setup(expression: x => x.GetIdsByFolderIds(It.Is<Guid[]>(ids => ids.Single() == folder.Id), true))
            .Returns(value: [file.Id]);
        fileContentServiceMock
            .Setup(expression: x => x.DeleteAllForFilesAsync(It.Is<Guid[]>(ids => ids.Single() == file.Id)))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await folderProcessingService.HandleFolderDeleteEventAsync(folder: folder);

        // Then
        folderServiceMock.Verify(expression: x => x.GetAll(true), times: Times.Exactly(2));
        fileServiceMock.Verify(
            expression: x => x.GetIdsByFolderIds(It.Is<Guid[]>(ids => ids.Single() == folder.Id), true),
            times: Times.Once
        );
        fileContentServiceMock.Verify(
            expression: x => x.DeleteAllForFilesAsync(It.Is<Guid[]>(ids => ids.Single() == file.Id)),
            times: Times.Once
        );
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

}