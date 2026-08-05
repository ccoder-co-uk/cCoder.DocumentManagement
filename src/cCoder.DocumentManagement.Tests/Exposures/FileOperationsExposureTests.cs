// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;

namespace cCoder.Core.Services.Tests.DMS.Exposures;

public partial class FileOperationsExposureTests
{
    [Fact]
    public async Task ShouldDelegateFileOperationsAsync()
    {
        // Given
        const int appId = 42;
        const string path = "/documents/report.pdf";
        Guid[] folderIds = [Guid.NewGuid(), Guid.NewGuid()];
        Guid[] fileIds = [Guid.NewGuid()];
        DataFile file = new();
        DataFile updatedFile = new();
        IQueryable<DataFile> files = new[] { file }.AsQueryable();
        Mock<IFileService> serviceMock = new(behavior: MockBehavior.Strict);

        serviceMock.Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: files);

        serviceMock.Setup(expression: service => service.GetByPathWithFolderAndContents(
            appId: appId, path: path, ignoreFilters: false))
            .Returns(value: file);

        serviceMock.Setup(expression: service => service.GetIdsByFolderIds(
            folderIds: folderIds, ignoreFilters: true))
            .Returns(value: fileIds);

        serviceMock.Setup(expression: service => service.UpdateFileAsync(updatedFile: updatedFile))
            .ReturnsAsync(value: file);

        serviceMock.Setup(expression: service => service.UpdateForAppFileAsync(updatedFile: updatedFile))
            .ReturnsAsync(value: file);

        FileOperationsExposure exposure = new(fileService: serviceMock.Object);

        // When
        IQueryable<DataFile> actualFiles = exposure.GetAllFiles(ignoreFilters: true);
        DataFile actualFile = exposure.GetFileByPathWithFolderAndContents(appId: appId, path: path);
        Guid[] actualIds = exposure.GetFileIdsByFolderIds(folderIds: folderIds, ignoreFilters: true);
        DataFile actualUpdate = await exposure.UpdateFileAsync(updatedFile: updatedFile);
        DataFile actualAppUpdate = await exposure.UpdateFileForAppAsync(updatedFile: updatedFile);

        // Then
        actualFiles.Should()
            .BeSameAs(expected: files);

        actualFile.Should()
            .BeSameAs(expected: file);

        actualIds.Should()
            .BeSameAs(expected: fileIds);

        actualUpdate.Should()
            .BeSameAs(expected: file);

        actualAppUpdate.Should()
            .BeSameAs(expected: file);

        serviceMock.VerifyAll();
    }
}