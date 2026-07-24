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
    File Get(Guid id);
    Guid[] GetIdsByFolderIds(Guid[] folderIds, bool ignoreFilters = false);
    File GetWithFolderAndContents(Guid id, bool ignoreFilters = false);
    File GetWithFolderRolesAndContents(Guid id, bool ignoreFilters = false);
    File GetByPath(int appId, string path, bool ignoreFilters = false);
    File GetByPathWithFolderAndContents(int appId, string path, bool ignoreFilters = false);
    File GetByPathWithFolderRolesAndContents(int appId, string path, bool ignoreFilters = false);
    IQueryable<File> Search(int appId, byte[] needle);
    IQueryable<File> GetAll(bool ignoreFilters = false);
    ValueTask<File> AddFileAsync(File entity);
    ValueTask<File> UpdateFileAsync(File entity);
    ValueTask<File> UpdateForAppFileAsync(File entity);
    ValueTask DeleteAsync(Guid id);
    ValueTask DeleteAllForAppFileAsync(IEnumerable<File> items);
}