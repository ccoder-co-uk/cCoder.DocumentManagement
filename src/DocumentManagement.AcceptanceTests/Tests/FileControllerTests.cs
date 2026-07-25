// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;
using DmsFile = cCoder.Data.Models.DMS.File;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.DocumentManagement;

[Collection(name: WebAcceptanceCollection.Name)]
public sealed partial class FileControllerTests(WebAcceptanceFixture fixture)
{
    private const int AppId = 1;
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/File";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFileContext(int AppId, Guid RoleId, Guid FolderId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFileContext> SeedDatabase(params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Role role = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = AppId,
            Name = Unique(prefix: "AcceptanceRole"),
            Description = "Acceptance role",
            Privs = string.Join(separator: ',', value: privileges),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = role.Id, UserId = "Guest" });

        Folder folder = await core.InsertFolderAsync(folder: new Folder
        {
            AppId = AppId,
            Name = Unique(prefix: "Folder"),
            Path = Unique(prefix: "folder")
            .ToLowerInvariant(),
        });

        Guid[] appRoleIds = core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: foundRole => foundRole.AppId == AppId)
            .Select(selector: foundRole => foundRole.Id)
            .ToArray();

        foreach (Guid roleId in appRoleIds)
        {
            await core.InsertFolderRoleAsync(folderRole: new FolderRole { FolderId = folder.Id, RoleId = roleId });
        }

        return new SeededFileContext(AppId: AppId, RoleId: role.Id, FolderId: folder.Id);
    }

    private async Task<DmsFile> CreateLocalFileAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<DmsFile>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateFileAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchFileAsync(Guid id, object payload)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Patch, requestUri: $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteFileAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<DmsFile> GetFileAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        if (content.Contains(value: "\"value\":[]", comparisonType: StringComparison.Ordinal))
        {
            return null;
        }

        return JsonSerializer.Deserialize<DmsFile>(json: content, options: JsonOptions);
    }

    private async Task Teardown(SeededFileContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FolderRole[] folderRoles = core.Set<FolderRole>()
            .IgnoreQueryFilters()
            .Where(predicate: folderRole => folderRole.FolderId == seededContext.FolderId)
            .ToArray();

        if (folderRoles.Length > 0)
        {
            await core.DeleteAllAsync(folderRoles: folderRoles);
        }

        FileContent[] contents = core.Set<FileContent>()
            .IgnoreQueryFilters()
            .Where(predicate: c => c.File.FolderId == seededContext.FolderId)
            .ToArray();

        if (contents.Length > 0)
        {
            await core.DeleteAllAsync(fileContents: contents);
        }

        DmsFile[] files = core.Set<DmsFile>()
            .IgnoreQueryFilters()
            .Where(predicate: file => file.FolderId == seededContext.FolderId)
            .ToArray();

        if (files.Length > 0)
        {
            await core.DeleteAllAsync(files: files);
        }

        Folder folder = core.Set<Folder>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.FolderId);

        if (folder is not null)
        {
            await core.DeleteAsync(folder: folder);
        }

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.RoleId == seededContext.RoleId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        Role role = core.Set<Role>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.RoleId);

        if (role is not null)
        {
            await core.DeleteAsync(role: role);
        }
    }

    private async Task<int> GetFileCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<DmsFile>> GetFilesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<DmsFile>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> GetFileStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}