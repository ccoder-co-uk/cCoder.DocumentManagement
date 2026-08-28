# cCoder.DocumentManagement

`cCoder.DocumentManagement` contains the Document Management domain for the cCoder platform. It provides folder, file, file-content, and folder-role functionality that can be consumed directly as a domain package or hosted through the standalone web app.

[View the latest main-branch code coverage report](https://ccoder-co-uk.github.io/cCoder.DocumentManagement/)

## Local Configuration

The Web app binds the complete configuration root to
`DocumentManagement.Web.Models.AppConfiguration`. The application composition
root then registers each required domain side by side: `CoreData`,
`DocumentManagement`, `SecurityData`, `Security`, and `Eventing`.

Persistence belongs to the Data domains. `DocumentManagement` contains only
Document Management behavior, while `CoreData` owns its database connection and
migrations. Likewise, `SecurityData` owns the Security database and `Security`
contains authentication behavior. Leave secret values empty in
`appsettings.json` and define these as user-level or machine-level environment
variables:

- `CoreData__ConnectionString`
- `SecurityData__ConnectionString`
- `Security__DecryptionKey`
- `Eventing__ServiceBus__ConnectionString` when `Eventing__ProviderType` is
  `ServiceBus`

`CoreData__AdminConnectionString` and
`SecurityData__AdminConnectionString` are optional migration-only overrides. If
an admin connection is configured, startup migrations use it and normal runtime
operations continue to use the regular connection. If it is omitted, migrations
use the regular connection.

Library consumers register persistence and behavior explicitly at their own
composition root: call `AddData`, `AddSecurityData`, `AddSecurityWeb`, and
`AddDocumentManagementWeb` with their matching configuration objects. An
application that consumes `cCoder.Core` should use Core's composite API instead;
Core deliberately composes its configured child domains recursively.

Restart Visual Studio, select the Web startup project, and press F5. No
configuration conversion step is required.

## Functionality

- Folder management: create and maintain application-owned folder trees with path hooks and optional parent folders.
- File management: manage files inside folders, including path, MIME type, description, size, and creator metadata.
- File content management: manage versioned content records under files.
- Folder roles: grant roles access to folders through the folder-role relationship.
- DMS and WebDAV endpoints: expose document operations through the web host for API and WebDAV-style access.
- Manual test UI: `/tools/index.html` provides a lightweight CRUD surface for folders and their children. Folders are managed as aggregate roots; files and folder roles are managed inside the selected folder, and file content is managed inside the selected file.
- Operational health: `/Health` returns `OK` for simple host checks.

## Contents

- `src/cCoder.DocumentManagement`
  The main library package published to NuGet.
- `src/DocumentManagement.Web`
  The standalone web host for the Document Management domain.
- `src/cCoder.DocumentManagement.Tests`
  Unit tests for the domain.
- `src/DocumentManagement.AcceptanceTests`
  Acceptance tests for the standalone host.

## Build

```powershell
dotnet build src/cCoder.DocumentManagement.slnx -v minimal
```

## Test

```powershell
dotnet test src/cCoder.DocumentManagement.slnx -v minimal --no-build
```

The solution test run includes unit and acceptance tests. Acceptance tests actively call the hosted HTTP surface, including `/Health`, the manual tools shell, OData endpoints, DMS middleware, and WebDAV middleware.

## Run Locally

```powershell
dotnet run --project src/DocumentManagement.Web/DocumentManagement.Web.csproj
```

Useful local endpoints:

- `/` redirects to `/tools/index.html`.
- `/tools/index.html` opens the manual domain tester.
- `/swagger` opens the API explorer.
- `/Health` returns `OK`.

## Package

The NuGet package produced by this repository is:

- `cCoder.DocumentManagement`

## Publishing

GitHub Actions is configured to publish the main package using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.DocumentManagement`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.