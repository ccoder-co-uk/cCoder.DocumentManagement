// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.Core.Services.Tests.DMS.Processings;

public partial class WebDavProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnMetadataXmlWhenMethodIsPropFindForFile()
    {
        // Given
        Folder folder = CreateFolderAsync(path: "folder");
        LocalFile file = CreateFileAsync(path: "folder/file.txt", folder: folder);
        DmsProcessingRequest request = CreateRequest(
            method: "PROPFIND",
            requestPath: "Core/App(7)/DAV/folder/file.txt",
            contentType: "application/xml",
            body: new MemoryStream(
                buffer: "<propfind xmlns=\"DAV:\"><prop><displayname /></prop></propfind>"u8.ToArray()
            )
        );
        fileServiceMock.Setup(expression: x => x.GetAll()).Returns(value: new[] { file }.AsQueryable());

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);
        string xml = ReadBodyText(stream: response.Body);

        // Then
        response.StatusCode.Should().Be(expected: 207);
        response.ContentType.Should().Be(expected: "text/xml; charset=\"utf-8\"");
        xml.Should().Contain(expected: "file.txt");
        xml.Should().Contain(expected: "Core/App(7)/DAV/folder/file.txt");
        fileServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnFolderMetadataXmlWhenMethodIsPropFindForRootFolder()
    {
        // Given
        Folder childFolder = CreateFolderAsync(path: "docs");
        DmsProcessingRequest request = CreateRequest(
            method: "PROPFIND",
            requestPath: "Core/App(7)/DAV",
            headers: new Dictionary<string, string[]>(comparer: StringComparer.OrdinalIgnoreCase)
            {
                [key: "Authorization"] = ["Basic token"],
                [key: "Depth"] = ["1"],
            }
        );
        folderServiceMock.Setup(expression: x => x.GetAll()).Returns(value: new[] { childFolder }.AsQueryable());
        fileServiceMock.Setup(expression: x => x.GetAll()).Returns(value: Array.Empty<LocalFile>().AsQueryable());

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request: request);
        string xml = ReadBodyText(stream: response.Body);

        // Then
        response.StatusCode.Should().Be(expected: 207);
        xml.Should().Contain(expected: "Root");
        xml.Should().Contain(expected: "docs");
        folderServiceMock.Verify(expression: x => x.GetAll(), times: Times.Exactly(2));
        fileServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}