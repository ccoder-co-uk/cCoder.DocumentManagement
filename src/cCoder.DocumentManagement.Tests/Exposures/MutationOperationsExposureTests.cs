// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Exposures;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Services.Aggregations;
using Moq;
using Xunit;
using DataFile = cCoder.Data.Models.DMS.File;
using DataFolder = cCoder.Data.Models.DMS.Folder;

namespace cCoder.DocumentManagement.Tests.Exposures;

public sealed partial class MutationOperationsExposureTests
{
    [Fact]
    public void FileGetAll_WhenCalled_DelegatesToMutationAggregation()
    {
        // Given
        Mock<IFileMutationAggregationService> serviceMock = new(behavior: MockBehavior.Strict);

        IQueryable<DataFile> expected = Array.Empty<DataFile>()
            .AsQueryable();

        serviceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: expected);

        FileMutationOperationsExposure exposure = new(fileMutationAggregationService: serviceMock.Object);

        // When
        _ = exposure.GetAllFiles(ignoreFilters: true);

        // Then
        serviceMock.VerifyAll();
    }

    [Fact]
    public async Task FileDelete_WhenCalled_DelegatesToMutationAggregationAsync()
    {
        // Given
        Mock<IFileMutationAggregationService> serviceMock = new(behavior: MockBehavior.Strict);
        Guid id = Guid.NewGuid();

        serviceMock
            .Setup(expression: service => service.DeleteAsync(fileId: id))
            .Returns(value: ValueTask.CompletedTask);

        FileMutationOperationsExposure exposure = new(fileMutationAggregationService: serviceMock.Object);

        // When
        await exposure.DeleteFileAsync(fileId: id);

        // Then
        serviceMock.VerifyAll();
    }

    [Fact]
    public void FolderGetAll_WhenCalled_DelegatesToMutationAggregation()
    {
        // Given
        Mock<IFolderMutationAggregationService> serviceMock = new(behavior: MockBehavior.Strict);

        IQueryable<DataFolder> expected = Array.Empty<DataFolder>()
            .AsQueryable();

        serviceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: expected);

        FolderMutationOperationsExposure exposure = new(folderMutationAggregationService: serviceMock.Object);

        // When
        _ = exposure.GetAllFolders(ignoreFilters: true);

        // Then
        serviceMock.VerifyAll();
    }

    [Fact]
    public async Task FolderAddOrUpdate_WhenCalled_DelegatesToMutationAggregationAsync()
    {
        // Given
        Mock<IFolderMutationAggregationService> serviceMock = new(behavior: MockBehavior.Strict);
        DataFolder[] folders = [new DataFolder()];
        IEnumerable<Result<DataFolder>> expected = [];

        serviceMock
            .Setup(expression: service => service.AddOrUpdateFolder(items: folders))
            .Returns(value: ValueTask.FromResult(result: expected));

        FolderMutationOperationsExposure exposure = new(folderMutationAggregationService: serviceMock.Object);

        // When
        _ = await exposure.AddOrUpdateFoldersAsync(folders: folders);

        // Then
        serviceMock.VerifyAll();
    }

    [Fact]
    public async Task AppFolderAddOrUpdate_WhenCalled_DelegatesToMutationAggregationAsync()
    {
        // Given
        Mock<IFolderMutationAggregationService> serviceMock = new(behavior: MockBehavior.Strict);
        DataFolder[] folders = [new DataFolder()];
        IEnumerable<Result<DataFolder>> expected = [];

        serviceMock
            .Setup(expression: service => service.AddOrUpdateForAppFolderAsync(items: folders))
            .Returns(value: ValueTask.FromResult(result: expected));

        FolderMutationOperationsExposure exposure = new(folderMutationAggregationService: serviceMock.Object);

        // When
        _ = await exposure.AddOrUpdateAppFoldersAsync(folders: folders);

        // Then
        serviceMock.VerifyAll();
    }

    [Fact]
    public async Task FolderDeleteAllByApp_WhenCalled_DelegatesToMutationAggregationAsync()
    {
        // Given
        Mock<IFolderMutationAggregationService> serviceMock = new(behavior: MockBehavior.Strict);

        serviceMock
            .Setup(expression: service => service.DeleteAllByAppIdAsync(appId: 7))
            .Returns(value: ValueTask.CompletedTask);

        FolderMutationOperationsExposure exposure = new(folderMutationAggregationService: serviceMock.Object);

        // When
        await exposure.DeleteAllByAppIdAsync(appId: 7);

        // Then
        serviceMock.VerifyAll();
    }
}