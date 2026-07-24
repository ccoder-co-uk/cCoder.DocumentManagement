// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Dependencies.DMSResult;
using DmsPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class DmsInstanceProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnBodyAndMimeTypeWhenGetReturnsResult()
    {
        // Given
        MemoryStream body = new(buffer: [1, 2, 3]);
        DMSResult result = new() { Data = body, MimeType = "text/plain" };

        DmsProcessingRequest request = CreateRequest(
            method: "GET",
            requestPath: "/api/dms/folder/file.txt",
            queryString: "?version=3&search=needle"
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.Get(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 3, search: "needle")
            )
            .Returns(value: result);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 200);

        response.ContentType.Should()
            .Be(expected: "text/plain");

        response.HasBody.Should()
            .BeTrue();

        response.Body.Should()
            .BeSameAs(expected: body);

        dmsInstanceServiceMock.Verify(
            expression: x => x.Get(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 3, search: "needle"),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldZipAndReturnOctetStreamWhenGetIncludesDownloadPaths()
    {
        // Given
        string[] expectedPaths = ["one.txt", "two.txt"];
        MemoryStream body = new(buffer: [9, 8, 7]);
        DMSResult result = new() { Data = body, MimeType = "application/zip" };

        DmsProcessingRequest request = CreateRequest(
            method: "GET",
            requestPath: "/api/dms/folder",
            queryString: "?download=true&downloadPaths=one.txt,two.txt"
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.GetFilesZipped(
                    paths: It.Is<IEnumerable<DmsPath>>(match: paths =>
                        paths.Select(selector: path => path.FullPath)
            .SequenceEqual(second: expectedPaths)
                    )
                )
            )
            .Returns(value: result);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 200);

        response.ContentType.Should()
            .Be(expected: "application/octet-stream");

        response.HasBody.Should()
            .BeTrue();

        response.Body.Should()
            .BeSameAs(expected: body);

        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.GetFilesZipped(
                    paths: It.Is<IEnumerable<DmsPath>>(match: paths =>
                        paths.Select(selector: path => path.FullPath)
            .SequenceEqual(second: expectedPaths)
                    )
                ),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenGetReturnsNull()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(method: "GET", requestPath: "/api/dms/folder/file.txt");

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.Get(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 0, search: string.Empty)
            )
            .Returns(value: (DMSResult)null!);

        // When
        DmsProcessingResponse response = await dmsProcessingService.ProcessDmsProcessingRequestAsync(request: request);

        // Then
        response.StatusCode.Should()
            .Be(expected: 204);

        response.ContentType.Should()
            .Be(expected: "plain/text");

        response.HasBody.Should()
            .BeFalse();

        dmsInstanceServiceMock.Verify(
            expression: x => x.Get(path: It.Is<DmsPath>(match: path => path.FullPath == "folder/file.txt"), version: 0, search: string.Empty),
            times: Times.Once
        );

        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}