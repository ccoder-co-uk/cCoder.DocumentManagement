// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Services.Foundations;

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

public interface IFileService
{
    File Get(Guid fileId);
    Guid[] GetIdsByFolderIds(Guid[] folderIds, bool ignoreFilters = false);
    File GetWithFolderAndContents(Guid fileId, bool ignoreFilters = false);
    File GetWithFolderRolesAndContents(Guid fileId, bool ignoreFilters = false);
    File GetByPath(int appId, string path, bool ignoreFilters = false);
    File GetByPathWithFolderAndContents(int appId, string path, bool ignoreFilters = false);
    File GetByPathWithFolderRolesAndContents(int appId, string path, bool ignoreFilters = false);
    IQueryable<File> Search(int appId, byte[] needle);
    IQueryable<File> GetAll(bool ignoreFilters = false);
    ValueTask<File> AddFileAsync(File newFile);
    ValueTask<File> UpdateFileAsync(File updatedFile);
    ValueTask<File> UpdateForAppFileAsync(File updatedFile);
    ValueTask DeleteAsync(Guid fileId);
    ValueTask DeleteAllForAppFileAsync(IEnumerable<File> deletedFile);
}