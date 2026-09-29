// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace cCoder.DocumentManagement.Exposures;

public interface IDmsHttpRequestManager
{
    ValueTask ProcessRequestAsync(HttpContext context);
}