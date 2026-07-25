// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models.Exceptions;

public sealed class DocumentManagementDependencyException(Exception innerException)
    : Exception(
        message: "A document management dependency failed.",
        innerException: innerException);