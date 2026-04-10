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
        Folder folder = CreateFolderAsync("folder");
        LocalFile file = CreateFileAsync("folder/file.txt", folder);
        DmsProcessingRequest request = CreateRequest(
            "PROPFIND",
            "Core/App(7)/DAV/folder/file.txt",
            contentType: "application/xml",
            body: new MemoryStream(
                "<propfind xmlns=\"DAV:\"><prop><displayname /></prop></propfind>"u8.ToArray()
            )
        );
        fileServiceMock.Setup(x => x.GetAll()).Returns(new[] { file }.AsQueryable());

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);
        string xml = ReadBodyText(response.Body);

        // Then
        response.StatusCode.Should().Be(207);
        response.ContentType.Should().Be("text/xml; charset=\"utf-8\"");
        xml.Should().Contain("file.txt");
        xml.Should().Contain("Core/App(7)/DAV/folder/file.txt");
        fileServiceMock.Verify(x => x.GetAll(), Times.Once);
        fileServiceMock.VerifyNoOtherCalls();
        folderServiceMock.VerifyNoOtherCalls();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnFolderMetadataXmlWhenMethodIsPropFindForRootFolder()
    {
        // Given
        Folder childFolder = CreateFolderAsync("docs");
        DmsProcessingRequest request = CreateRequest(
            "PROPFIND",
            "Core/App(7)/DAV",
            headers: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["Authorization"] = ["Basic token"],
                ["Depth"] = ["1"],
            }
        );
        folderServiceMock.Setup(x => x.GetAll()).Returns(new[] { childFolder }.AsQueryable());
        fileServiceMock.Setup(x => x.GetAll()).Returns(Array.Empty<LocalFile>().AsQueryable());

        // When
        DmsProcessingResponse response = await webDavProcessingService.ProcessAsync(request);
        string xml = ReadBodyText(response.Body);

        // Then
        response.StatusCode.Should().Be(207);
        xml.Should().Contain("Root");
        xml.Should().Contain("docs");
        folderServiceMock.Verify(x => x.GetAll(), Times.Exactly(2));
        fileServiceMock.Verify(x => x.GetAll(), Times.Once);
        folderServiceMock.VerifyNoOtherCalls();
        fileServiceMock.VerifyNoOtherCalls();
        dmsInstanceServiceMock.VerifyNoOtherCalls();
    }
}









