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
using DMSResult = cCoder.DocumentManagement.Models.DMSResult;
using DataFile = cCoder.Data.Models.DMS.File;
using ExternalPath = cCoder.DocumentManagement.Models.Path;
using LocalFile = cCoder.Data.Models.DMS.File;
using LocalPath = cCoder.DocumentManagement.Models.Path;


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
        DMSResult result = orchestrationService.GetFilesZippedDmsOperation(
            operation: new DmsOperation
            {
                Paths = paths.Select(
                    selector: path =>
                        path.FullPath)
            })
            .Result;

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
        DMSResult result = orchestrationService.GetDmsOperation(
            operation: new DmsOperation
            {
                Path = path.FullPath,
                Version = 2
            })
            .Result;

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
        DMSResult result = orchestrationService.GetDmsOperation(
            operation: new DmsOperation
            {
                Path = path.FullPath,
                Search = "needle"
            })
            .Result;

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
        IEnumerable<DataFile> result = orchestrationService.SearchFilesDmsOperation(
            operation: new DmsOperation
            {
                Needle = "needle"
            })
            .Files;

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
        _ = await orchestrationService.UnpackDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = path.FullPath,
                Content = stream,
                IgnoreArchiveRoot = true
            });

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
        _ = await orchestrationService.SaveDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = path.FullPath,
                Content = stream
            });

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
        _ = await orchestrationService.SaveDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = path.FullPath
            });

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
        _ = await orchestrationService.DropDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = path.FullPath,
                Version = 2
            });

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
        _ = await orchestrationService.DropDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = path.FullPath
            });

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
        _ = await orchestrationService.CopyDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = oldPath.FullPath,
                NewPath = newPath.FullPath
            });

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
        _ = await orchestrationService.CopyDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = oldPath.FullPath,
                NewPath = newPath.FullPath
            });

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
        _ = await orchestrationService.MoveDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = oldPath.FullPath,
                NewPath = newPath.FullPath
            });

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
        _ = await orchestrationService.MoveDmsOperationAsync(
            operation: new DmsOperation
            {
                Path = oldPath.FullPath,
                NewPath = newPath.FullPath
            });

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