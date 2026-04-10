using System.Security;
using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsProcessingServiceTests
{
    [Fact]
    public async Task ShouldRethrowWhenDmsInstanceThrowsSecurityException()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("DELETE", "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessAsync(request);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        dmsInstanceServiceMock.Verify(
            x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRethrowWhenDmsInstanceThrowsException()
    {
        // Given
        DmsProcessingRequest request = CreateRequest("GET", "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(x =>
                x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0, string.Empty)
            )
            .Throws(new InvalidOperationException("Boom"));

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessAsync(request);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Boom");
        dmsInstanceServiceMock.Verify(
            x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0, string.Empty),
            Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}







