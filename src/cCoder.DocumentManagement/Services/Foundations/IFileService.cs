// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cCoder.DocumentManagement.Services.Foundations;

using cCoder.Data.Models.DMS;

internal interface IFileService
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