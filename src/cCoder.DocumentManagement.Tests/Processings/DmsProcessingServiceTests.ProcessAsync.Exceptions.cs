// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        DmsProcessingRequest request = CreateRequest(method: "DELETE", requestPath: "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0))
            .Throws(exception: new SecurityException("Access Denied!"));

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessAsync(request: request);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        dmsInstanceServiceMock.Verify(
            expression: x => x.DropAsync(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRethrowWhenDmsInstanceThrowsException()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "GET", requestPath: "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0, string.Empty)
            )
            .Throws(exception: new InvalidOperationException("Boom"));

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessAsync(request: request);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(expectedWildcardPattern: "Boom");
        dmsInstanceServiceMock.Verify(
            expression: x => x.Get(It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"), 0, string.Empty),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}