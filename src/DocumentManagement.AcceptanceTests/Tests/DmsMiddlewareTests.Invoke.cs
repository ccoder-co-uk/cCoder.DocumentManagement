// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using cCoder.Data.Models.DMS;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;
using DmsFile = cCoder.Data.Models.DMS.File;


namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class DmsMiddlewareTests
{
    [Fact]
    public async Task Invoke_HandlesOptionsRequest()
    {
        // Given
        (int StatusCode, IReadOnlyCollection<string> Headers) actualResult;

        // When
        actualResult = await InvokeOptionsAsync();

        // Then
        actualResult.StatusCode.Should()
            .Be(expected: (int)HttpStatusCode.NoContent);

        actualResult.Headers.Should()
            .Contain(expected: "Access-Control-Allow-Origin");

        actualResult.Headers.Should()
            .Contain(expected: "Access-Control-Allow-Methods");
    }

    [Fact]
    public async Task Invoke_ReturnsLatestFileContentWhenVersionIsNotSpecified()
    {
        // Given
        string unique = Guid.NewGuid()
            .ToString(format: "N");

        string folderPath = $"latest-version-{unique}";
        string filePath = $"{folderPath}/document.txt";
        Guid folderId;
        Guid fileId;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            Folder folder = await core.InsertFolderAsync(folder: new Folder
            {
                AppId = 1,
                Name = folderPath,
                Path = folderPath,
            });

            DmsFile file = await core.InsertFileAsync(file: new DmsFile
            {
                FolderId = folder.Id,
                Name = "document.txt",
                Path = filePath,
                MimeType = "text/plain",
            });

            _ = await core.InsertFileContentAsync(fileContent: new FileContent
            {
                FileId = file.Id,
                Version = 2,
                RawData = Encoding.UTF8.GetBytes(s: "latest"),
            });

            _ = await core.InsertFileContentAsync(fileContent: new FileContent
            {
                FileId = file.Id,
                Version = 1,
                RawData = Encoding.UTF8.GetBytes(s: "oldest"),
            });

            folderId = folder.Id;
            fileId = file.Id;
        }

        string actualContent;

        // When
        using (HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/{filePath}"))
        {
            actualContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should()
                .Be(expected: HttpStatusCode.OK, because: actualContent);
        }

        // Then
        actualContent.Should()
            .Be(expected: "latest");

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            FileContent[] contents = await core.Set<FileContent>()
                .IgnoreQueryFilters()
                .Where(predicate: content => content.FileId == fileId)
                .ToArrayAsync();

            core.RemoveRange(entities: contents);

            DmsFile file = await core.Set<DmsFile>()
                .IgnoreQueryFilters()
                .SingleAsync(predicate: found => found.Id == fileId);

            core.Remove(entity: file);

            Folder folder = await core.Set<Folder>()
                .IgnoreQueryFilters()
                .SingleAsync(predicate: found => found.Id == folderId);

            core.Remove(entity: folder);
            _ = await core.SaveChangesAsync();
        }
    }
}