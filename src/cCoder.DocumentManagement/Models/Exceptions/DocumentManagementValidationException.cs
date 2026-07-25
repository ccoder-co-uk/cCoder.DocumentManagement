// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Models.Exceptions;

public sealed class DocumentManagementValidationException(Exception innerException)
    : Exception(
        message: "Document management validation failed.",
        innerException: innerException);