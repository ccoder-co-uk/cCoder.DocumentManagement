// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Services.Processings;


namespace cCoder.DocumentManagement.Exposures;

public interface IDmsHttpRequestManager
{
    ValueTask ProcessRequestAsync(HttpContext context);
}