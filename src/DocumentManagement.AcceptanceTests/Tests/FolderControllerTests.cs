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
public sealed partial class FolderControllerTests(WebAcceptanceFixture fixture)
{
    private const int AppId = 1;
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/Folder";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFolderContext(int AppId, Guid RoleId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFolderContext> SeedDatabase(params string[] privileges)
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

        return new SeededFolderContext(AppId, role.Id);
    }

    private async Task<Folder> CreateFolderAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<Folder>(content, JsonOptions)!;
    }

    private async Task<int> UpdateFolderAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchFolderAsync(Guid id, object payload)
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

    private async Task<int> DeleteFolderAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<Folder> GetFolderAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return null;

        if (content.Contains("\"value\":[]", StringComparison.Ordinal))
            return null;

        return JsonSerializer.Deserialize<Folder>(content, JsonOptions);
    }

    private async Task Teardown(SeededFolderContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FolderRole[] folderRoles = core.Set<FolderRole>().IgnoreQueryFilters().Where(folderRole => folderRole.Folder.AppId == seededContext.AppId).ToArray();
        if (folderRoles.Length > 0)
            await core.DeleteAllAsync(folderRoles);

        FileContent[] contents = core.Set<FileContent>().IgnoreQueryFilters().Where(c => c.File.Folder.AppId == seededContext.AppId).ToArray();
        if (contents.Length > 0)
            await core.DeleteAllAsync(contents);

        DmsFile[] files = core.Set<DmsFile>().IgnoreQueryFilters().Where(file => file.Folder.AppId == seededContext.AppId).ToArray();
        if (files.Length > 0)
            await core.DeleteAllAsync(files);

        Folder[] folders = core.Set<Folder>().IgnoreQueryFilters().Where(folder => folder.AppId == seededContext.AppId).ToArray();
        if (folders.Length > 0)
            await core.DeleteAllAsync(folders);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.RoleId == seededContext.RoleId).ToArray();
        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.RoleId);
        if (role is not null)
            await core.DeleteAsync(role);
    }

    private async Task<int> GetFolderCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<Folder>> GetFoldersAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<Folder>>(content, JsonOptions)!.Value;
    }

    private async Task<int> CopyFolderAsync(string sourcePath, string destinationPath, int sourceAppId, int destinationAppId)
    {
        using HttpResponseMessage response = await Client.PostAsync(
            $"{BaseUrl}/Copy?source={Uri.EscapeDataString(sourcePath)}&destination={Uri.EscapeDataString(destinationPath)}&sourceAppId={sourceAppId}&destAppId={destinationAppId}",
            content: null);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<SeededFolderContext> SeedCopyDatabase(params string[] privileges)
        => await SeedDatabase(privileges);
    private async Task<int> GetFolderStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}







