# cCoder.DocumentManagement

`cCoder.DocumentManagement` contains the Document Management domain for the cCoder platform. It provides folder, file, file-content, and folder-role functionality that can be consumed directly as a domain package or hosted through the standalone web app.

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
dotnet build src/cCoder.DocumentManagement.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.DocumentManagement.sln -v minimal --no-build
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

## Local Configuration

The standalone web host reads local secrets from environment variables rather than committed config.

Before running `src/DocumentManagement.Web`, set:

- `ConnectionStrings__Core`
- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`

The committed `appsettings.json` keeps these values blank so user or machine environment variables can supply them during local development.

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
