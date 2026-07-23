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
        var app = CreateRandomApp();
        ExternalPath[] paths = [new(path: "/folder/")];
        DMSResult expected = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.GetFilesZipped(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    paths: It.Is<IEnumerable<LocalPath>>(match: items =>
                        items.Select(selector: item => item.FullPath)
            .SequenceEqual(second: paths.Select(selector: path => path.FullPath))
                    )
                )
            )
            .Returns(valueFunction: () => expected);

        DMSResult result = orchestrationService.GetFilesZipped(paths: paths);

        result.Should()
            .BeSameAs(expected: expected);

        currentAppResolverMock.Verify(expression: x => x.ResolveCurrentApp(), times: Times.Once);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.GetFilesZipped(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
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
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/file.txt");
        DMSResult expected = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.Get(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                )
            )
            .Returns(valueFunction: () => expected);

        DMSResult result = orchestrationService.Get(path: path, version: 2);

        result.Should()
            .BeSameAs(expected: expected);

        fileProcessingServiceMock.Verify(
            expression: x =>
                x.Get(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                ),
            times: Times.Once
        );
    }

    [Fact]
    public void Get_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");
        DMSResult expected = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.Get(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    search: "needle"
                )
            )
            .Returns(valueFunction: () => expected);

        DMSResult result = orchestrationService.Get(path: path, search: "needle");

        result.Should()
            .BeSameAs(expected: expected);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.Get(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    search: "needle"
                ),
            times: Times.Once
        );
    }

    [Fact]
    public void Search_ShouldReturnFileProcessingResults()
    {
        var app = CreateRandomApp();
        LocalFile[] files = [new() { Id = Guid.NewGuid(), Name = "file.txt", Path = "file.txt" }];

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x => x.Search(app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name), needle: "needle"))
            .Returns(value: files);

        IEnumerable<DataFile> result = orchestrationService.Search(needle: "needle");

        result.Should()
            .ContainSingle();

        result.Single().Id.Should()
            .Be(expected: files[0].Id);

        result.Single().Name.Should()
            .Be(expected: files[0].Name);

        result.Single().Path.Should()
            .Be(expected: files[0].Path);

        fileProcessingServiceMock.Verify(
            expression: x => x.Search(app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name), needle: "needle"),
            times: Times.Once
        );
    }

    [Fact]
    public async Task UnpackAsync_ShouldDelegateToFolderProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");
        using MemoryStream stream = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.UnpackAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    content: stream,
                    ignoreArchiveRoot: true
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.UnpackAsync(path: path, content: stream, ignoreArchiveRoot: true);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.UnpackAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
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
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/file.txt");
        using MemoryStream stream = new();

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.SaveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    content: stream
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.SaveAsync(path: path, content: stream);

        fileProcessingServiceMock.Verify(
            expression: x =>
                x.SaveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    content: stream
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task SaveAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.SaveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.SaveAsync(path: path);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.SaveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task DropAsync_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/file.txt");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.DropAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.DropAsync(path: path, version: 2);

        fileProcessingServiceMock.Verify(
            expression: x =>
                x.DropAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath),
                    version: 2
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task DropAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath path = new(path: "/folder/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.DropAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.DropAsync(path: path);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.DropAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    path: It.Is<LocalPath>(match: item => item.FullPath == path.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task CopyAsync_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/file.txt");
        ExternalPath newPath = new(path: "/copy.txt");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.CopyAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.CopyAsync(oldPath: oldPath, newPath: newPath);

        fileProcessingServiceMock.Verify(
            expression: x =>
                x.CopyAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task CopyAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/folder/");
        ExternalPath newPath = new(path: "/copy/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.CopyAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.CopyAsync(oldPath: oldPath, newPath: newPath);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.CopyAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task MoveAsync_WhenPathIsFile_ShouldUseFileProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/file.txt");
        ExternalPath newPath = new(path: "/moved.txt");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        fileProcessingServiceMock
            .Setup(expression: x =>
                x.MoveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.MoveAsync(oldPath: oldPath, newPath: newPath);

        fileProcessingServiceMock.Verify(
            expression: x =>
                x.MoveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }

    [Fact]
    public async Task MoveAsync_WhenPathIsFolder_ShouldUseFolderProcessingService()
    {
        var app = CreateRandomApp();
        ExternalPath oldPath = new(path: "/folder/");
        ExternalPath newPath = new(path: "/moved/");

        currentAppResolverMock.Setup(expression: x => x.ResolveCurrentApp())
            .Returns(value: app);

        folderProcessingServiceMock
            .Setup(expression: x =>
                x.MoveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                )
            )
            .Returns(value: ValueTask.CompletedTask);

        await orchestrationService.MoveAsync(oldPath: oldPath, newPath: newPath);

        folderProcessingServiceMock.Verify(
            expression: x =>
                x.MoveAsync(
                    app: It.Is<App>(match: a => a.Id == app.Id && a.Name == app.Name),
                    oldPath: It.Is<LocalPath>(match: item => item.FullPath == oldPath.FullPath),
                    newPath: It.Is<LocalPath>(match: item => item.FullPath == newPath.FullPath)
                ),
            times: Times.Once
        );
    }
}