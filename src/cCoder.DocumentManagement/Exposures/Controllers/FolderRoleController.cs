// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Api.OData;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.DocumentManagement.Exposures.Controllers;

public class FolderRoleController : ODataController
{
    protected IFolderRoleOrchestrationService Service { get; }

    public FolderRoleController(IFolderRoleOrchestrationService service, ILogger<FolderRoleController> log)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        return Ok(value: new MetadataContainer(type: typeof(FolderRole), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        return Ok(value: Service.GetAll());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FolderRole entity)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: base.ModelState);
        }

        return Ok(value: await Service.AddFolderRoleAsync(entity: entity));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] ODataCollection<FolderRole> items)
    {
        if (!base.ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: base.ModelState);
        }

        await Service.DeleteAllFolderRoleAsync(items: items.Value);
        return Ok();
    }
}