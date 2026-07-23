// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using DmsFile = cCoder.Data.Models.DMS.File;

using Microsoft.EntityFrameworkCore;
using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderControllerTests
{
    [Fact]
    public async Task Delete_RemovesChildFoldersFilesAndContents()
    {
        // Given
        SeededFolderContext seededContext = await SeedDatabase("folder_delete", "file_delete");
        Guid rootFolderId;
        Guid childFolderId;
        Guid fileId;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            Folder rootFolder = await core.AddFolderAsync(folder: new Folder
            {
                Id = Guid.NewGuid(),
                AppId = seededContext.AppId,
                Name = Unique(prefix: "Root"),
                Path = Unique(prefix: "root")
                .ToLowerInvariant()
            });

            Folder childFolder = await core.AddFolderAsync(folder: new Folder
            {
                Id = Guid.NewGuid(),
                AppId = seededContext.AppId,
                ParentId = rootFolder.Id,
                Name = Unique(prefix: "Child"),
                Path = $"{rootFolder.Path}/{Unique(prefix: "child")
                .ToLowerInvariant()}"
            });

            await core.AddFolderRoleAsync(folderRole: new FolderRole { FolderId = rootFolder.Id, RoleId = seededContext.RoleId });
            await core.AddFolderRoleAsync(folderRole: new FolderRole { FolderId = childFolder.Id, RoleId = seededContext.RoleId });

            DmsFile file = await core.AddDmsFileAsync(file: new DmsFile
            {
                Id = Guid.NewGuid(),
                FolderId = childFolder.Id,
                Name = "file.txt",
                Path = $"{childFolder.Path}/file.txt",
                MimeType = "text/plain",
                CreatedBy = "Guest",
                CreatedOn = DateTimeOffset.UtcNow,
                Size = "1 B"
            });

            await core.AddFileContentAsync(fileContent: new FileContent
            {
                Id = Guid.NewGuid(),
                FileId = file.Id,
                Description = "content",
                Size = "1 B",
                CreatedBy = "Guest",
                CreatedOn = DateTimeOffset.UtcNow,
                Version = 1,
                RawData = [1]
            });

            rootFolderId = rootFolder.Id;
            childFolderId = childFolder.Id;
            fileId = file.Id;
        }

        // When
        int actualStatusCode = await DeleteFolderAsync(id: rootFolderId);

        // Then
        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            actualStatusCode.Should()
                .Be(expected: 200);

            core.Set<Folder>()
                .IgnoreQueryFilters()
                .Any(predicate: folder => folder.Id == rootFolderId)
                .Should()
                .BeFalse();

            core.Set<Folder>()
                .IgnoreQueryFilters()
                .Any(predicate: folder => folder.Id == childFolderId)
                .Should()
                .BeFalse();

            core.Set<DmsFile>()
                .IgnoreQueryFilters()
                .Any(predicate: file => file.Id == fileId)
                .Should()
                .BeFalse();

            core.Set<FileContent>()
                .IgnoreQueryFilters()
                .Any(predicate: content => content.FileId == fileId)
                .Should()
                .BeFalse();
        }

        await Teardown(seededContext: seededContext);
    }
}