// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

public interface IFolderRoleProcessingService
{
    IQueryable<FolderRole> GetAll(bool ignoreFilters = false);

    ValueTask<FolderRole> AddAsync(FolderRole entity);

    ValueTask DeleteAsync(FolderRole entity);

    ValueTask<IEnumerable<Result<FolderRole>>> AddOrUpdate(IEnumerable<FolderRole> items);

    ValueTask DeleteAllAsync(IEnumerable<FolderRole> items);
}