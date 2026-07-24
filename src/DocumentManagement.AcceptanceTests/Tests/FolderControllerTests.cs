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
public sealed partial class FolderControllerTests(WebAcceptanceFixture fixture)
{
    private const int AppId = 1;
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/Folder";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFolderContext(int AppId, Guid RoleId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFolderContext> SeedDatabase(params string[] privileges)
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

        return new SeededFolderContext(AppId: AppId, RoleId: role.Id);
    }

    private async Task<Folder> CreateFolderAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<Folder>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateFolderAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchFolderAsync(Guid id, object payload)
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

    private async Task<int> DeleteFolderAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<Folder> GetFolderAsync(Guid id)
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

        return JsonSerializer.Deserialize<Folder>(json: content, options: JsonOptions);
    }

    private async Task Teardown(SeededFolderContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FolderRole[] folderRoles = core.Set<FolderRole>()
            .IgnoreQueryFilters()
            .Where(predicate: folderRole => folderRole.Folder.AppId == seededContext.AppId)
            .ToArray();

        if (folderRoles.Length > 0)
        {
            await core.DeleteAllAsync(folderRoles: folderRoles);
        }

        FileContent[] contents = core.Set<FileContent>()
            .IgnoreQueryFilters()
            .Where(predicate: c => c.File.Folder.AppId == seededContext.AppId)
            .ToArray();

        if (contents.Length > 0)
        {
            await core.DeleteAllAsync(fileContents: contents);
        }

        DmsFile[] files = core.Set<DmsFile>()
            .IgnoreQueryFilters()
            .Where(predicate: file => file.Folder.AppId == seededContext.AppId)
            .ToArray();

        if (files.Length > 0)
        {
            await core.DeleteAllAsync(files: files);
        }

        Folder[] folders = core.Set<Folder>()
            .IgnoreQueryFilters()
            .Where(predicate: folder => folder.AppId == seededContext.AppId)
            .ToArray();

        if (folders.Length > 0)
        {
            await core.DeleteAllAsync(folders: folders);
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

    private async Task<int> GetFolderCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<Folder>> GetFoldersAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<Folder>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<int> CopyFolderAsync(string sourcePath, string destinationPath, int sourceAppId, int destinationAppId)
    {
        using HttpResponseMessage response = await Client.PostAsync(
            requestUri: $"{BaseUrl}/Copy?source={Uri.EscapeDataString(stringToEscape: sourcePath)}&destination={Uri.EscapeDataString(stringToEscape: destinationPath)}&sourceAppId={sourceAppId}&destAppId={destinationAppId}",
            content: null);

        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private Task<SeededFolderContext> SeedCopyDatabase(params string[] privileges) =>
        SeedDatabase(privileges: privileges);
    private async Task<int> GetFolderStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}
