// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models.Exceptions;

public sealed class DuplicateFolderRoleException()
    : Exception(message: "The folder is already in the selected role.");