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
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        cCoder.Data.Models.DMS.File file = CreateRandomFile(appId: 3, path: "root/file.txt");
        fileServiceMock.Setup(x => x.GetByPath(3, "root/file.txt", true)).Returns(file);

        // When
        cCoder.Data.Models.DMS.File result = fileProcessingService.GetByPath(
            3,
            "root/file.txt"
        );

        // Then
        result.Should().BeEquivalentTo(file);
        fileServiceMock.Verify(x => x.GetByPath(3, "root/file.txt", true), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenFileDoesNotExistForGetByPath()
    {
        // Given
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        fileServiceMock.Setup(x => x.GetByPath(3, "root/file.txt", true)).Returns((cCoder.Data.Models.DMS.File)null);

        // When
        Action act = () => fileProcessingService.GetByPath(3, "root/file.txt");

        // Then
        act.Should().Throw<SecurityException>().WithMessage("Access Denied!");
        fileServiceMock.Verify(x => x.GetByPath(3, "root/file.txt", true), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
    }

}









