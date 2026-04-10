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
        folderServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { folder }.AsQueryable());
        fileServiceMock
            .Setup(x => x.GetIdsByFolderIds(It.Is<Guid[]>(ids => ids.Single() == folder.Id), true))
            .Returns([file.Id]);
        fileContentServiceMock
            .Setup(x => x.DeleteAllForFilesAsync(It.Is<Guid[]>(ids => ids.Single() == file.Id)))
            .Returns(ValueTask.CompletedTask);

        // When
        await folderProcessingService.HandleFolderDeleteEventAsync(folder);

        // Then
        folderServiceMock.Verify(x => x.GetAll(true), Times.Exactly(2));
        fileServiceMock.Verify(
            x => x.GetIdsByFolderIds(It.Is<Guid[]>(ids => ids.Single() == folder.Id), true),
            Times.Once
        );
        fileContentServiceMock.Verify(
            x => x.DeleteAllForFilesAsync(It.Is<Guid[]>(ids => ids.Single() == file.Id)),
            Times.Once
        );
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        fileContentServiceMock.VerifyNoOtherCalls();
        loggerMock.VerifyNoOtherCalls();
    }

}









