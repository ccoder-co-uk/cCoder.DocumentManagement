// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public void ShouldReturnMatchingFileWhenFileExistsForGetByPath()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                {
                    throw new SecurityException("Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

        cCoder.Data.Models.DMS.File file = CreateRandomFile(appId: 3, path: "root/file.txt");
        fileServiceMock.Setup(expression: x => x.GetByPath(3, "root/file.txt", true)).Returns(value: file);

        // When
        cCoder.Data.Models.DMS.File result = fileProcessingService.GetByPath(
            appId: 3,
            path: "root/file.txt"
        );

        // Then
        result.Should().BeEquivalentTo(expectation: file);
        fileServiceMock.Verify(expression: x => x.GetByPath(3, "root/file.txt", true), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenFileDoesNotExistForGetByPath()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                {
                    throw new SecurityException("Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(valueFunction: () => currentUser);

        fileServiceMock.Setup(expression: x => x.GetByPath(3, "root/file.txt", true)).Returns(value: (cCoder.Data.Models.DMS.File)null);

        // When
        Action act = () => fileProcessingService.GetByPath(appId: 3, path: "root/file.txt");

        // Then
        act.Should().Throw<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        fileServiceMock.Verify(expression: x => x.GetByPath(3, "root/file.txt", true), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}