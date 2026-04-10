using cCoder.DocumentManagement.Services.Processings;
using DataFile = cCoder.Data.Models.DMS.File;
using DmsResult = cCoder.DocumentManagement.Models.DMSResult;
using LocalApp = cCoder.Data.Models.CMS.App;
using LocalFile = cCoder.Data.Models.DMS.File;
using LocalPath = cCoder.DocumentManagement.Models.Path;


namespace cCoder.DocumentManagement.Services.Orchestrations;

internal class DmsOrchestrationService(
    IDocumentManagementCurrentAppResolver currentAppResolver,
    IFileProcessingService fileProcessingService,
    IFolderProcessingService folderProcessingService
) : IDmsOrchestrationService
{
    public DmsResult GetFilesZipped(IEnumerable<LocalPath> paths)
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        return folderProcessingService.GetFilesZipped(app, paths);
    }

    public DmsResult Get(LocalPath path, int version = 0, string search = "")
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        return path.IsToFile
            ? fileProcessingService.Get(app, path, version)
            : folderProcessingService.Get(app, path, search);
    }

    public IEnumerable<DataFile> Search(string needle)
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        return fileProcessingService.Search(app, needle).Select(ToExternalFile).ToArray();
    }

    public async ValueTask UnpackAsync(LocalPath path, Stream content, bool ignoreArchiveRoot = false)
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        await folderProcessingService.UnpackAsync(app, path, content, ignoreArchiveRoot);
    }

    public async ValueTask SaveAsync(LocalPath path, Stream content = null)
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        if (path.IsToFile)
            await fileProcessingService.SaveAsync(app, path, content);
        else
            await folderProcessingService.SaveAsync(app, path);
    }

    public async ValueTask DropAsync(LocalPath path, int version = 0)
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        if (path.IsToFile)
            await fileProcessingService.DropAsync(app, path, version);
        else
            await folderProcessingService.DropAsync(app, path);
    }

    public async ValueTask CopyAsync(LocalPath oldPath, LocalPath newPath)
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        if (oldPath.IsToFile)
            await fileProcessingService.CopyAsync(app, oldPath, newPath);
        else
            await folderProcessingService.CopyAsync(app, oldPath, newPath);
    }

    public async ValueTask MoveAsync(LocalPath oldPath, LocalPath newPath)
    {
        LocalApp app = currentAppResolver.ResolveCurrentApp();
        if (oldPath.IsToFile)
            await fileProcessingService.MoveAsync(app, oldPath, newPath);
        else
            await folderProcessingService.MoveAsync(app, oldPath, newPath);
    }
    private static DataFile ToExternalFile(LocalFile file) => file == null ? null : new DataFile
    {
        Id = file.Id,
        FolderId = file.FolderId,
        Name = file.Name,
        Description = file.Description,
        Path = file.Path,
        MimeType = file.MimeType,
        CreatedBy = file.CreatedBy,
        Size = file.Size,
        CreatedOn = file.CreatedOn,
        DeletedOn = file.DeletedOn,
    };
}