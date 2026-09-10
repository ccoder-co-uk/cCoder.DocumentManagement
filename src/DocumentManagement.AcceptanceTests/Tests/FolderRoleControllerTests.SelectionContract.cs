// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Web.AcceptanceTests.Tests.DocumentManagement;

public sealed partial class FolderRoleControllerTests
{
    [Theory]
    [InlineData("RoleId")]
    [InlineData("FolderId")]
    public async Task Get_FiltersExistingFolderRoles(string filterProperty)
    {
        // Given
        var seeded = await SeedDatabase(includeFolderRole: true, privileges: ["app_admin", "folder_read", "folderrole_create", "folderrole_delete"]);

        try
        {
            // When
            Guid filterId = filterProperty == "RoleId" ? seeded.RoleId : seeded.FolderId;
            using var response = await Client.GetAsync(requestUri: $"{BaseUrl}?$filter={filterProperty} eq {filterId}");
            string body = await response.Content.ReadAsStringAsync();
            // Then
            response.StatusCode.Should()
                .Be(expected: HttpStatusCode.OK, because: body);

            var rows = JsonSerializer.Deserialize<ODataEnvelope<FolderRole>>(json: body, options: JsonOptions)!.Value;

            rows.Should()
                .ContainSingle(predicate: row => row.FolderId == seeded.FolderId && row.RoleId == seeded.RoleId);

            rows.Should()
                .OnlyContain(predicate: row => filterProperty == "RoleId" ? row.RoleId == filterId : row.FolderId == filterId);
        }
        finally { await Teardown(seededContext: seeded); }
    }

    [Fact]
    public async Task Post_ExistingFolderRoleReturnsConflictWithoutChangingRows()
    {
        // Given
        var seeded = await SeedDatabase(includeFolderRole: true, privileges: ["app_admin", "folder_read", "folderrole_create", "folderrole_delete"]);

        try
        {
            // When
            using var response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: new { seeded.FolderId, seeded.RoleId });
            string body = await response.Content.ReadAsStringAsync();
            // Then
            using var scope = fixture.Factory.Services.CreateScope();

            using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            var rows = await core.Set<FolderRole>()
                .IgnoreQueryFilters()
                .Where(predicate: row => row.FolderId == seeded.FolderId)
                .ToArrayAsync();

            rows.Should()
                .HaveCount(expected: 2);

            rows.Should()
                .ContainSingle(predicate: row => row.RoleId == seeded.RoleId);

            response.StatusCode.Should()
                .Be(expected: HttpStatusCode.Conflict, because: body);

            body.Should()
                .ContainEquivalentOf(expected: "already");

            body.Should()
                .ContainEquivalentOf(expected: "role");
        }
        finally { await Teardown(seededContext: seeded); }
    }

    [Fact]
    public async Task Delete_CompositeKeyRemovesOnlySelectedFolderRole()
    {
        // Given
        var seeded = await SeedDatabase(includeFolderRole: true, privileges: ["app_admin", "folder_read", "folderrole_create", "folderrole_delete"]);

        try
        {
            // When
            using var response = await Client.DeleteAsync(requestUri: $"{BaseUrl}(FolderId={seeded.FolderId},RoleId={seeded.RoleId})");
            string body = await response.Content.ReadAsStringAsync();
            // Then
            response.StatusCode.Should()
                .Be(expected: HttpStatusCode.NoContent, because: body);

            using var scope = fixture.Factory.Services.CreateScope();

            using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
                .CreateCoreContext();

            var rows = await core.Set<FolderRole>()
                .IgnoreQueryFilters()
                .Where(predicate: row => row.FolderId == seeded.FolderId)
                .ToArrayAsync();

            rows.Should()
                .ContainSingle().Which.RoleId.Should()
                .Be(expected: seeded.AccessRoleId);
        }
        finally { await Teardown(seededContext: seeded); }
    }
}