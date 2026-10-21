// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Text;
using cCoder.Data.Models.DMS;
using cCoder.DocumentManagement.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class FileProcessingServiceTests
{
    [Fact]
    public void ShouldReturnLatestFileContentWhenVersionIsNotSpecified()
    {
        // Given
        const int appId = 3;
        const string path = "root/file.txt";
        cCoder.Data.Models.DMS.File file = CreateRandomFile(appId: appId, path: path);

        file.Contents =
        [
            new FileContent { FileId = file.Id, Version = 3, RawData = Encoding.UTF8.GetBytes(s: "latest") },
            new FileContent { FileId = file.Id, Version = 1, RawData = Encoding.UTF8.GetBytes(s: "oldest") },
            new FileContent { FileId = file.Id, Version = 2, RawData = Encoding.UTF8.GetBytes(s: "middle") },
        ];

        fileServiceMock.Setup(expression: service => service.GetByPathWithFolderAndContents(
                appId: appId,
                path: path,
                ignoreFilters: false))
            .Returns(value: file);

        // When
        DMSResult result = fileProcessingService.GetAppPath(appId: appId, path: path, version: 0);

        // Then
        using StreamReader reader = new(stream: result.Data, encoding: Encoding.UTF8);

        reader.ReadToEnd()
            .Should()
            .Be(expected: "latest");

        fileServiceMock.Verify(expression: service => service.GetByPathWithFolderAndContents(
            appId: appId,
            path: path,
            ignoreFilters: false), times: Times.Once);

        fileServiceMock.VerifyNoOtherCalls();
    }
}