// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.Security;

namespace cCoder.DocumentManagement.Services.Processings;

internal interface IFolderRoleProcessingService
{
    IQueryable<FolderRole> GetAll(bool ignoreFilters = false);

    ValueTask<FolderRole> AddFolderRoleAsync(FolderRole newFolderRole);

    ValueTask DeleteFolderRoleAsync(FolderRole deletedFolderRole);

    ValueTask<IEnumerable<Result<FolderRole>>> AddOrUpdateFolderRole(IEnumerable<FolderRole> items);

    ValueTask DeleteAllFolderRoleAsync(IEnumerable<FolderRole> deletedFolderRole);
}