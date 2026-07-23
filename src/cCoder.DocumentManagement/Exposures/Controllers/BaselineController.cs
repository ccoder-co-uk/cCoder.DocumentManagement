// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Exposures.Setup;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.DocumentManagement.Exposures.Controllers;

[ApiController]
[Route("Api/DocumentManagement/Baseline")]
public sealed class BaselineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: UIBaseline.Packages);
}