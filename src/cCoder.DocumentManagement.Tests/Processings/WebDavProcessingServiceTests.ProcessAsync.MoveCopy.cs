// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using DmsPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToMoveWhenMethodIsMove()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "MOVE",
            requestPath: "Core/App(7)/DAV/folder/file.txt",
            headers: new Dictionary<string, string[]>(comparer: StringComparer.OrdinalIgnoreCase)
            {
                [key: "Authorization"] = ["Basic token"],
                [key: "Destination"] =
                [
                    "https://example.test/Api/Core/App(7)/DAV/folder/archive/file.txt",
                ],
            }
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.MoveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.MoveAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseNormalizedDestinationPathWhenMethodIsCopy()
    {
        // Given
        DmsProcessingRequest request = CreateRequest(
            method: "COPY",
            requestPath: "Core/App(7)/DAV/folder/file.txt",
            headers: new Dictionary<string, string[]>(comparer: StringComparer.OrdinalIgnoreCase)
            {
                [key: "Authorization"] = ["Basic token"],
                [key: "Destination"] =
                [
                    "https://example.test/Api/Core/App(7)/DAV/folder/archive/file.txt",
                ],
            }
        );

        dmsInstanceServiceMock
            .Setup(expression: x =>
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);

        // Then
        response.StatusCode.Should().Be(expected: 204);
        dmsInstanceServiceMock.Verify(
            expression: x =>
                x.CopyAsync(
                    It.Is<DmsPath>(path => path.FullPath == "folder/file.txt"),
                    It.Is<DmsPath>(path => path.FullPath == "folder/archive/file.txt")
                ),
            times: Times.Once
        );
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}