// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Foundations;
using DataFile = cCoder.Data.Models.DMS.File;

namespace cCoder.DocumentManagement.Exposures;

internal sealed class FileOperationsExposure(
    IFileService fileService)
    : IFileOperationsExposure
{
    public IQueryable<DataFile> GetAllFiles(bool ignoreFilters = false) =>
        fileService.GetAll(
            ignoreFilters: ignoreFilters);

    public DataFile GetFileByPathWithFolderAndContents(
        int appId,
        string path) =>
        fileService.GetByPathWithFolderAndContents(
            appId: appId,
            path: path);

    public Guid[] GetFileIdsByFolderIds(
        IEnumerable<Guid> folderIds,
        bool ignoreFilters) =>
        fileService.GetIdsByFolderIds(
            folderIds: folderIds.ToArray(),
            ignoreFilters: ignoreFilters);

    public ValueTask<DataFile> UpdateFileAsync(DataFile updatedFile) =>
        fileService.UpdateFileAsync(
            updatedFile: updatedFile);

    public ValueTask<DataFile> UpdateFileForAppAsync(DataFile updatedFile) =>
        fileService.UpdateForAppFileAsync(
            updatedFile: updatedFile);

}