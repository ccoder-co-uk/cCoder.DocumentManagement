// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace cCoder.DocumentManagement.Exposures.Controllers;

[ApiController]
[Route("Api/DocumentManagement/Baseline")]
public sealed class BaselineController(IBaselineExposure baselineExposure) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: baselineExposure.GetBaselinePackages());
}