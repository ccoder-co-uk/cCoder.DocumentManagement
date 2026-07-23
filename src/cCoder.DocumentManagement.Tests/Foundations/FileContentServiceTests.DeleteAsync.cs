// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(value: new User { Id = "test-user" });
        Guid fileContentId = Guid.NewGuid();
        FileContent fileContent = CreateRandomFileContent(id: fileContentId);

        fileContentBrokerMock
            .Setup(expression: x => x.GetAllFileContents(false))
            .Returns(value: new[] { ToExternalFileContent(fileContent) }.AsQueryable());

        fileContentBrokerMock.Setup(expression: x => x.GetAppId(It.IsAny<DataFileContent>())).Returns(value: (int?)7);
        authorizationBrokerMock.Setup(expression: x => x.Authorize((int?)7, "FileContent_delete"));
        fileContentBrokerMock
            .Setup(expression: x => x.DeleteFileContentAsync(It.IsAny<DataFileContent>()))
            .ReturnsAsync(value: 1);

        // When
        await fileContentService.DeleteAsync(id: fileContentId);

        // Then
        fileContentBrokerMock.Verify(expression: x => x.GetAllFileContents(false), times: Times.Once);
        fileContentBrokerMock.Verify(
            expression: x => x.DeleteFileContentAsync(It.Is<DataFileContent>(candidate => candidate.Id == fileContent.Id)),
            times: Times.Once
        );
        fileContentBrokerMock.Verify(
            expression: x => x.GetAppId(It.IsAny<DataFileContent>()),
            times: Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize((int?)7, "FileContent_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid fileContentId = Guid.NewGuid();
        FileContent fileContent = CreateRandomFileContent(id: fileContentId);

        fileContentBrokerMock
            .Setup(expression: x => x.GetAllFileContents(false))
            .Returns(value: new[] { ToExternalFileContent(fileContent) }.AsQueryable());

        fileContentBrokerMock.Setup(expression: x => x.GetAppId(It.IsAny<DataFileContent>())).Returns(value: (int?)7);
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize((int?)7, "FileContent_delete"))
            .Throws(exception: new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await fileContentService.DeleteAsync(id: fileContentId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        fileContentBrokerMock.Verify(expression: x => x.GetAllFileContents(false), times: Times.Once);
        fileContentBrokerMock.Verify(
            expression: x => x.GetAppId(It.IsAny<DataFileContent>()),
            times: Times.AtMostOnce()
        );
        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize((int?)7, "FileContent_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}