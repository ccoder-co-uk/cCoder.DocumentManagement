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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: CreateAuthorizedUser(appId: 7, privilege: "file_delete"));

        Guid fileId = Guid.NewGuid();
        FileEntity file = CreateRandomFile(id: fileId);

        fileBrokerMock
            .Setup(expression: x => x.SelectAllFiles(ignoreFilters: true))
            .Returns(value: new[] { ToExternalFile(file: file) }.AsQueryable());

        fileBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()))
            .Returns(value: (int?)7);

        fileBrokerMock.Setup(expression: x => x.DeleteFileAsync(entity: It.IsAny<DataFile>()))
            .ReturnsAsync(value: 1);

        // When
        await fileService.DeleteAsync(fileId: fileId);

        // Then
        fileBrokerMock.Verify(expression: x => x.SelectAllFiles(ignoreFilters: true), times: Times.Once);

        fileBrokerMock.Verify(
            expression: x => x.DeleteFileAsync(entity: It.Is<DataFile>(match: candidate => candidate.Id == file.Id)),
            times: Times.Once
        );

        fileBrokerMock.Verify(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()), times: Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid fileId = Guid.NewGuid();
        FileEntity file = CreateRandomFile(id: fileId);

        fileBrokerMock
            .Setup(expression: x => x.SelectAllFiles(ignoreFilters: true))
            .Returns(value: new[] { ToExternalFile(file: file) }.AsQueryable());

        fileBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: null);

        // When
        Func<Task> action = async () => await fileService.DeleteAsync(fileId: fileId);

        // Then
        await action.Should()
            .ThrowAsync<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(SecurityException));

        fileBrokerMock.Verify(expression: x => x.SelectAllFiles(ignoreFilters: true), times: Times.Once);
        fileBrokerMock.Verify(expression: x => x.SelectAppId(entity: It.IsAny<DataFile>()), times: Times.AtMostOnce());
        fileBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.GetCurrentUser(), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}