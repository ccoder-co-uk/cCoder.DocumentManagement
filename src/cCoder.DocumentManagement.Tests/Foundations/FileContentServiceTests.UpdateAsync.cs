using System.Security;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        FileContent fileContent = CreateRandomFileContent();

        FileContent submitted = null;

        fileContentBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFileContent>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FileContent_update"));

        fileContentBrokerMock
            .Setup(x => x.UpdateFileContentAsync(It.IsAny<DataFileContent>()))
            .Callback<DataFileContent>(candidate =>
                submitted = new FileContent
                {
                    Id = candidate.Id,
                    FileId = candidate.FileId,
                    Description = candidate.Description,
                    Size = candidate.Size,
                    CreatedBy = candidate.CreatedBy,
                    CreatedOn = candidate.CreatedOn,
                    Version = candidate.Version,
                    RawData = candidate.RawData,
                }
            )
            .ReturnsAsync((DataFileContent value) => value);

        // When
        FileContent result = await fileContentService.UpdateAsync(fileContent);

        // Then
        result.Should().BeSameAs(fileContent);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(fileContent);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(fileContent);
        result.Should().BeEquivalentTo(fileContent);
        fileContentBrokerMock.Verify(
            x => x.UpdateFileContentAsync(It.IsAny<DataFileContent>()),
            Times.Once
        );
        fileContentBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataFileContent>()),
            Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FileContent_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();

        fileContentBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataFileContent>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FileContent_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileContentService.UpdateAsync(fileContent);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        fileContentBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataFileContent>()),
            Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "FileContent_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








