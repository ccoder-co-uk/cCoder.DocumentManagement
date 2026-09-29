// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.Security;


namespace cCoder.DocumentManagement.Services.Foundations;

internal interface IRoleService
{
    IQueryable<Role> GetAll(bool ignoreFilters = false);
}