// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

[Collection(name: WebAcceptanceCollection.Name)]
public sealed partial class FolderRoleControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/FolderRole";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFolderRoleContext(int AppId, Guid FolderId, Guid AccessRoleId, Guid RoleId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFolderRoleContext> SeedDatabase(bool includeFolderRole = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "folderrole")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role accessRole = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "AccessRole"),
            Description = "Acceptance access role",
            Privs = string.Join(separator: ',', value: privileges),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = accessRole.Id, UserId = "Guest" });

        Role role = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "TargetRole"),
            Description = "Acceptance target role",
            Privs = "folder_read",
        });

        Folder folder = await core.InsertFolderAsync(folder: new Folder
        {
            AppId = app.Id,
            Name = Unique(prefix: "Folder"),
            Path = Unique(prefix: "folder")
            .ToLowerInvariant(),
        });

        await core.InsertFolderRoleAsync(folderRole: new FolderRole
        {
            FolderId = folder.Id,
            RoleId = accessRole.Id,
        });

        if (includeFolderRole)
        {
            await core.InsertFolderRoleAsync(folderRole: new FolderRole
            {
                FolderId = folder.Id,
                RoleId = role.Id,
            });
        }

        return new SeededFolderRoleContext(AppId: app.Id, FolderId: folder.Id, AccessRoleId: accessRole.Id, RoleId: role.Id);
    }

    private async Task<FolderRole> CreateFolderRoleAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<FolderRole>(json: content, options: JsonOptions)!;
    }

    private async Task Teardown(SeededFolderRoleContext seededContext)
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

        Folder folder = core.Set<Folder>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.FolderId);

        if (folder is not null)
        {
            await core.DeleteAsync(folder: folder);
        }

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.RoleId == seededContext.AccessRoleId)
            .ToArray();

        if (userRoles.Length > 0)
        {
            await core.DeleteAllAsync(userRoles: userRoles);
        }

        Role[] roles = core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: found => found.Id == seededContext.AccessRoleId || found.Id == seededContext.RoleId)
            .ToArray();

        if (roles.Length > 0)
        {
            await core.DeleteAllAsync(roles: roles);
        }

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: found => found.Id == seededContext.AppId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }
    }


    private async Task<int> GetFolderRoleCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<FolderRole>> GetFolderRolesAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<FolderRole>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<FolderRole> FindFolderRoleAsync(Guid folderId, Guid roleId)
    {
        IReadOnlyList<FolderRole> folderRoles = await GetFolderRolesAsync(top: 200);

        return folderRoles.FirstOrDefault(predicate: folderRole =>
            folderRole.FolderId == folderId && folderRole.RoleId == roleId
        );
    }

}