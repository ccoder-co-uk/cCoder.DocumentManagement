# cCoder.DocumentManagement

`cCoder.DocumentManagement` contains the Document Management domain for the cCoder platform.

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
