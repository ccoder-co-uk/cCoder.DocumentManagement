using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        FileEntity file = CreateRandomFile();

        FileEntity submitted = null;

        fileBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFile>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "File_update"));

        fileBrokerMock
            .Setup(x => x.UpdateFileAsync(It.IsAny<DataFile>()))
            .Callback<DataFile>(candidate =>
                submitted = new FileEntity
                {
                    Id = candidate.Id,
                    FolderId = candidate.FolderId,
                    Name = candidate.Name,
                    Description = candidate.Description,
                    Path = candidate.Path,
                    MimeType = candidate.MimeType,
                    CreatedBy = candidate.CreatedBy,
                    CreatedOn = candidate.CreatedOn,
                    Size = candidate.Size,
                }
            )
            .ReturnsAsync((DataFile value) => value);

        // When
        FileEntity result = await fileService.UpdateAsync(file);

        // Then
        result.Should().BeSameAs(file);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(file);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(file);
        result.Should().BeEquivalentTo(file);
        fileBrokerMock.Verify(x => x.UpdateFileAsync(It.IsAny<DataFile>()), Times.Once);
        fileBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFile>()), Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "File_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FileEntity file = CreateRandomFile();

        fileBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFile>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "File_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileService.UpdateAsync(file);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        fileBrokerMock.Verify(x => x.GetAppId(It.IsAny<DataFile>()), Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "File_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}









