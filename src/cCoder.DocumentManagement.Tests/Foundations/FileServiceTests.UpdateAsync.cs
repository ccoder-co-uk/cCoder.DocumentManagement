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
using DataFile = cCoder.Data.Models.DMS.File;
using FileEntity = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FileServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        FileEntity file = CreateRandomFile();

        FileEntity submitted = null;

        fileBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "File_update"));

        fileBrokerMock
            .Setup(expression: x => x.UpdateFileAsync(entity: It.IsAny<DataFile>()))
            .Callback<DataFile>(action: candidate =>
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
            .ReturnsAsync(valueFunction: (DataFile value) => value);

        // When
        FileEntity result = await fileService.UpdateFileAsync(file: file);

        // Then
        result.Should()
            .BeSameAs(expected: file);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: file);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: file);

        result.Should()
            .BeEquivalentTo(expectation: file);

        fileBrokerMock.Verify(expression: x => x.UpdateFileAsync(entity: It.IsAny<DataFile>()), times: Times.Once);
        fileBrokerMock.Verify(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()), times: Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "File_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FileEntity file = CreateRandomFile();

        fileBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "File_update"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await fileService.UpdateFileAsync(file: file);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        fileBrokerMock.Verify(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()), times: Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "File_update"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}