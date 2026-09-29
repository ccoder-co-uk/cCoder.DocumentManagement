// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Models.Exceptions;

using System.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsInstanceProcessingServiceTests
{
    [Fact]
    public async Task ShouldRethrowWhenDmsInstanceThrowsSecurityException()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "DELETE", requestPath: "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(expression: x => x.DropAsync(path: It.Is<string>(match: path => path == "folder/file.txt"), version: 0))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        await act.Should()
            .ThrowAsync<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(SecurityException));

        dmsInstanceServiceMock.Verify(
            expression: x => x.DropAsync(path: It.Is<string>(match: path => path == "folder/file.txt"), version: 0),
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
                x.Get(path: It.Is<string>(match: path => path == "folder/file.txt"), version: 0, search: string.Empty)
            )
            .Throws(exception: new InvalidOperationException(message: "Boom"));

        // When
        Func<Task> act = async () => await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        await act.Should()
            .ThrowAsync<DocumentManagementServiceException>()
            .WithInnerException(innerException: typeof(InvalidOperationException));

        dmsInstanceServiceMock.Verify(
            expression: x => x.Get(path: It.Is<string>(match: path => path == "folder/file.txt"), version: 0, search: string.Empty),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}