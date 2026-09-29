// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.DocumentManagement.Models.Exceptions;

public sealed class DocumentManagementServiceException(Exception innerException)
    : Exception(
        message: "The document management service failed.",
        innerException: innerException);