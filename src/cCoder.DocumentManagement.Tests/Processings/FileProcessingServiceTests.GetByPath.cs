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
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        cCoder.Data.Models.DMS.File file = CreateRandomFile(appId: 3, path: "root/file.txt");

        fileServiceMock.Setup(expression: x => x.GetByPath(appId: 3, path: "root/file.txt", ignoreFilters: true))
            .Returns(value: file);

        // When
        cCoder.Data.Models.DMS.File result = fileProcessingService.GetByPath(
            appId: 3,
            path: "root/file.txt"
        );

        // Then
        result.Should()
            .BeEquivalentTo(expectation: file);

        fileServiceMock.Verify(expression: x => x.GetByPath(appId: 3, path: "root/file.txt", ignoreFilters: true), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenFileDoesNotExistForGetByPath()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        fileServiceMock.Setup(expression: x => x.GetByPath(appId: 3, path: "root/file.txt", ignoreFilters: true))
            .Returns(value: (cCoder.Data.Models.DMS.File)null);

        // When
        Action act = () => fileProcessingService.GetByPath(appId: 3, path: "root/file.txt");

        // Then
        act.Should()
            .Throw<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        fileServiceMock.Verify(expression: x => x.GetByPath(appId: 3, path: "root/file.txt", ignoreFilters: true), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}