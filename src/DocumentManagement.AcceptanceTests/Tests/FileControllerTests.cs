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

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class FileControllerTests(WebAcceptanceFixture fixture)
{
    private const int AppId = 1;
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/File";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFileContext(int AppId, Guid RoleId, Guid FolderId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFileContext> SeedDatabase(params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Role role = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = AppId,
            Name = Unique("AcceptanceRole"),
            Description = "Acceptance role",
            Privs = string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = role.Id, UserId = "Guest" });

        Folder folder = await core.AddFolderAsync(new Folder
        {
            AppId = AppId,
            Name = Unique("Folder"),
            Path = Unique("folder").ToLowerInvariant(),
        });

        Guid[] appRoleIds = core.Set<Role>().IgnoreQueryFilters()
            .Where(foundRole => foundRole.AppId == AppId)
            .Select(foundRole => foundRole.Id)
            .ToArray();

        foreach (Guid roleId in appRoleIds)
            await core.AddFolderRoleAsync(new FolderRole { FolderId = folder.Id, RoleId = roleId });

        return new SeededFileContext(AppId, role.Id, folder.Id);
    }

    private async Task<DmsFile> CreateFileAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<DmsFile>(content, JsonOptions)!;
    }

    private async Task<int> UpdateFileAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchFileAsync(Guid id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };
        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteFileAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<DmsFile> GetFileAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<DmsFile>(content, JsonOptions);
    }

    private async Task Teardown(SeededFileContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FolderRole[] folderRoles = core.Set<FolderRole>().IgnoreQueryFilters()
            .Where(folderRole => folderRole.FolderId == seededContext.FolderId)
            .ToArray();
        if (folderRoles.Length > 0)
            await core.DeleteAllAsync(folderRoles);

        FileContent[] contents = core.Set<FileContent>().IgnoreQueryFilters().Where(c => c.File.FolderId == seededContext.FolderId).ToArray();
        if (contents.Length > 0)
            await core.DeleteAllAsync(contents);

        DmsFile[] files = core.Set<DmsFile>().IgnoreQueryFilters().Where(file => file.FolderId == seededContext.FolderId).ToArray();
        if (files.Length > 0)
            await core.DeleteAllAsync(files);

        Folder folder = core.Set<Folder>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.FolderId);
        if (folder is not null)
            await core.DeleteAsync(folder);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.RoleId == seededContext.RoleId).ToArray();
        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.RoleId);
        if (role is not null)
            await core.DeleteAsync(role);
    }

    private async Task<int> GetFileCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<DmsFile>> GetFilesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<DmsFile>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetFileStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}







