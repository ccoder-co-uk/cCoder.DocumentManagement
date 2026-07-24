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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        FileContent fileContent = CreateRandomFileContent();

        FileContent submitted = null;

        fileContentBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_update"));

        fileContentBrokerMock
            .Setup(expression: x => x.UpdateFileContentAsync(entity: It.IsAny<DataFileContent>()))
            .Callback<DataFileContent>(action: candidate =>
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
            .ReturnsAsync(valueFunction: (DataFileContent value) => value);

        // When
        FileContent result = await fileContentService.UpdateFileContentAsync(fileContent: fileContent);

        // Then
        result.Should()
            .BeSameAs(expected: fileContent);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: fileContent);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: fileContent);

        result.Should()
            .BeEquivalentTo(expectation: fileContent);

        fileContentBrokerMock.Verify(
            expression: x => x.UpdateFileContentAsync(entity: It.IsAny<DataFileContent>()),
            times: Times.Once
        );

        fileContentBrokerMock.Verify(
            expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()),
            times: Times.AtMostOnce()
        );

        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FileContent fileContent = CreateRandomFileContent();

        fileContentBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await fileContentService.UpdateFileContentAsync(fileContent: fileContent);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        fileContentBrokerMock.Verify(
            expression: x => x.GetAppId(entity: It.IsAny<DataFileContent>()),
            times: Times.AtMostOnce()
        );

        fileContentBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "FileContent_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}