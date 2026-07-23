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
using DataFolder = cCoder.Data.Models.DMS.Folder;


namespace cCoder.Core.Services.Tests.DMS.Foundations;

public partial class FolderServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        Folder folder = CreateRandomFolder(appId: 7);

        Folder submitted = null;

        folderBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataFolder>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_create"));

        folderBrokerMock
            .Setup(expression: x =>
                x.InsertFolderAsync(entity: It.Is<DataFolder>(match: candidate => !ReferenceEquals(objA: candidate, objB: folder)))
            )
            .Callback<DataFolder>(action: candidate =>
                submitted = new Folder
                {
                    Id = candidate.Id,
                    AppId = candidate.AppId,
                    ParentId = candidate.ParentId,
                    Name = candidate.Name,
                    Path = candidate.Path,
                    DeletedOn = candidate.DeletedOn,
                }
            )
            .ReturnsAsync(valueFunction: (DataFolder value) => value);

        // When
        Folder result = await folderService.AddAsync(folder: folder);

        // Then
        result.Should()
            .BeSameAs(expected: folder);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: folder);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: folder, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: folder, config: options => options.Excluding(expression: candidate => candidate.Id));

        folderBrokerMock.Verify(
            expression: x => x.InsertFolderAsync(entity: It.Is<DataFolder>(match: candidate => !ReferenceEquals(objA: candidate, objB: folder))),
            times: Times.Once
        );

        folderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<DataFolder>()), times: Times.AtMostOnce());
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        Folder folder = CreateRandomFolder(appId: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await folderService.AddAsync(folder: folder);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        folderBrokerMock.Verify(expression: x => x.GetAppId(entity: It.IsAny<DataFolder>()), times: Times.AtMostOnce());
        folderBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)7, privilege: "Folder_create"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}