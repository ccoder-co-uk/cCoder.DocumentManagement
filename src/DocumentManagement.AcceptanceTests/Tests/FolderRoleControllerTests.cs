using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.DocumentManagement;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class FolderRoleControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/FolderRole";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFolderRoleContext(int AppId, Guid FolderId, Guid AccessRoleId, Guid RoleId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFolderRoleContext> SeedDatabase(bool includeFolderRole = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("folderrole")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role accessRole = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("AccessRole"),
            Description = "Acceptance access role",
            Privs = string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = accessRole.Id, UserId = "Guest" });

        Role role = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("TargetRole"),
            Description = "Acceptance target role",
            Privs = "folder_read",
        });

        Folder folder = await core.AddFolderAsync(new Folder
        {
            AppId = app.Id,
            Name = Unique("Folder"),
            Path = Unique("folder").ToLowerInvariant(),
        });

        await core.AddFolderRoleAsync(new FolderRole
        {
            FolderId = folder.Id,
            RoleId = accessRole.Id,
        });

        if (includeFolderRole)
        {
            await core.AddFolderRoleAsync(new FolderRole
            {
                FolderId = folder.Id,
                RoleId = role.Id,
            });
        }

        return new SeededFolderRoleContext(app.Id, folder.Id, accessRole.Id, role.Id);
    }

    private async Task<FolderRole> CreateFolderRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<FolderRole>(content, JsonOptions)!;
    }

    private async Task Teardown(SeededFolderRoleContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FolderRole[] folderRoles = core.Set<FolderRole>().IgnoreQueryFilters().Where(folderRole => folderRole.FolderId == seededContext.FolderId).ToArray();
        if (folderRoles.Length > 0)
            await core.DeleteAllAsync(folderRoles);

        Folder folder = core.Set<Folder>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.FolderId);
        if (folder is not null)
            await core.DeleteAsync(folder);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.RoleId == seededContext.AccessRoleId).ToArray();
        if (userRoles.Length > 0)
            await core.DeleteAllAsync(userRoles);

        Role[] roles = core.Set<Role>().IgnoreQueryFilters().Where(found => found.Id == seededContext.AccessRoleId || found.Id == seededContext.RoleId).ToArray();
        if (roles.Length > 0)
            await core.DeleteAllAsync(roles);

        App app = core.Set<App>().IgnoreQueryFilters().FirstOrDefault(found => found.Id == seededContext.AppId);
        if (app is not null)
            await core.DeleteAsync(app);
    }


    private async Task<int> GetFolderRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<FolderRole>> GetFolderRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<FolderRole>>(content, JsonOptions)!.Value;
    }

    private async Task<FolderRole> FindFolderRoleAsync(Guid folderId, Guid roleId)
    {
        IReadOnlyList<FolderRole> folderRoles = await GetFolderRolesAsync(200);
        return folderRoles.FirstOrDefault(folderRole =>
            folderRole.FolderId == folderId && folderRole.RoleId == roleId
        );
    }

}







