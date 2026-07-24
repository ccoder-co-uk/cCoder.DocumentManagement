// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DMSResult = cCoder.DocumentManagement.Dependencies.DMSResult;
using DataFile = cCoder.Data.Models.DMS.File;
using ExternalPath = cCoder.DocumentManagement.Dependencies.Path;
using LocalFile = cCoder.Data.Models.DMS.File;
using LocalPath = cCoder.DocumentManagement.Dependencies.Path;


namespace cCoder.Core.Services.Tests.DMS.Orchestrations;

public partial class DmsOrchestrationServiceTests
{
    [Fact]
    public void GetFilesZipped_ShouldDelegateToFolderProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath[] paths = [new(path: "/folder/")];
        DMSResult expected = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.GetFilesZippedAppPath(
                    appId: app.Id,
                    paths: It.Is<IEnumerable<LocalPath>>(match: items =>
                        items.Select(selector: item => item.FullPath)
            .SequenceEqual(second: paths.Select(selector: path => path.FullPath))
                    )
                )
            )
            .Returns(valueFunction: () => expected);

        // When
        DMSResult result = orchestrationService.GetFilesZipped(paths: paths);

        // Then
        result.Should()
            .BeSameAs(expected: expected);

        currentAppResolverMock.Verify(expression: x => x.ResolveCurrentApp(), times: Times.Once);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.GetFilesZippedAppPath(
                    appId: app.Id,
                    paths: It.Is<IEnumerable<LocalPath>>(match: items =>
                        items.Select(selector: item => item.FullPath)
            .SequenceEqual(second: paths.Select(selector: path => path.FullPath))
                    )
                ),
            times: Times.Once
        );
    }

    [Fact]
    public void Get_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/file.txt");
        DMSResult expected = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.GetAppPath(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                )
            )
            .Returns(valueFunction: () => expected);

        // When
        DMSResult result = orchestrationService.Get(path: path, version: 2);

        // Then
        result.Should()
            .BeSameAs(expected: expected);

        fileProcessingServiceMock.Verify(
            expression: x =>
                x.GetAppPath(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                ),
            times: Times.Once
        );
    }

    [Fact]
    public void Get_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");
        DMSResult expected = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.GetAppPath(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    search: "needle"
                )
            )
            .Returns(valueFunction: () => expected);

        // When
        DMSResult result = orchestrationService.Get(path: path, search: "needle");

        // Then
        result.Should()
            .BeSameAs(expected: expected);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.GetAppPath(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    search: "needle"
                ),
            times: Times.Once
        );
    }

    [Fact]
    public void Search_ShouldReturnFileProcessingResults()
    {
        // Given
        var app = CreateRandomApp();
        LocalFile[] files = [new() { Id = Guid.NewGuid(), Name = "file.txt", Path = "file.txt" }];

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x => x.SearchApp(appId: app.Id, needle: "needle"))
            .Returns(value: files);

        // When
        IEnumerable<DataFile> result = orchestrationService.Search(needle: "needle");

        // Then
        result.Should()
            .ContainSingle();

        result.Single().Id.Should()
            .Be(expected: files[0].Id);

        result.Single().Name.Should()
            .Be(expected: files[0].Name);

        result.Single().Path.Should()
            .Be(expected: files[0].Path);

        fileProcessingServiceMock.Verify(
            expression: x => x.SearchApp(appId: app.Id, needle: "needle"),
            times: Times.Once
        );
    }

    [Fact]
    public async Task UnpackAsync_ShouldDelegateToFolderProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");
        using MemoryStream stream = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.UnpackAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    content: stream,
                    ignoreArchiveRoot: true
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.UnpackAsync(path: path, content: stream, ignoreArchiveRoot: true);

        // Then
        folderProcessingServiceMock.Verify(
            expression: x =>
                x.UnpackAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    content: stream,
                    ignoreArchiveRoot: true
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task SaveAsync_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/file.txt");
        using MemoryStream stream = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.SaveAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    content: stream
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.SaveAsync(path: path, content: stream);

        // Then
        fileProcessingServiceMock.Verify(
            expression: x =>
                x.SaveAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    content: stream
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task SaveAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.SaveAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.SaveAsync(path: path);

        // Then
        folderProcessingServiceMock.Verify(
            expression: x =>
                x.SaveAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task DropAsync_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/file.txt");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.DropAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DropAsync(path: path, version: 2);

        // Then
        fileProcessingServiceMock.Verify(
            expression: x =>
                x.DropAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task DropAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.DropAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DropAsync(path: path);

        // Then
        folderProcessingServiceMock.Verify(
            expression: x =>
                x.DropAppPathAsync(
                    appId: app.Id,
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task CopyAsync_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/file.txt");
        ExternalPath newPath = new(path: "/copy.txt");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.CopyAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.CopyAsync(oldPath: oldPath, newPath: newPath);

        // Then
        fileProcessingServiceMock.Verify(
            expression: x =>
                x.CopyAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task CopyAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/folder/");
        ExternalPath newPath = new(path: "/copy/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.CopyAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.CopyAsync(oldPath: oldPath, newPath: newPath);

        // Then
        folderProcessingServiceMock.Verify(
            expression: x =>
                x.CopyAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task MoveAsync_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/file.txt");
        ExternalPath newPath = new(path: "/moved.txt");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.MoveAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.MoveAsync(oldPath: oldPath, newPath: newPath);

        // Then
        fileProcessingServiceMock.Verify(
            expression: x =>
                x.MoveAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task MoveAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        // Given
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/folder/");
        ExternalPath newPath = new(path: "/moved/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.MoveAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.MoveAsync(oldPath: oldPath, newPath: newPath);

        // Then
        folderProcessingServiceMock.Verify(
            expression: x =>
                x.MoveAppPathAsync(
                    appId: app.Id,
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }
}
