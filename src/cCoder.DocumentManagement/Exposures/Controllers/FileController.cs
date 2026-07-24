// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Api.OData;
using cCoder.Data.Extensions;
using cCoder.DocumentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using LocalFile = cCoder.Data.Models.DMS.File;


namespace cCoder.DocumentManagement.Exposures.Controllers;

public partial class FileController : ODataController
{
    protected IFileOrchestrationService Service { get; }

    public FileController(IFileOrchestrationService service, ILogger<FileController> log)
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query[key: "extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                value: new cCoder.DocumentManagement.Api.OData.DocumentManagementModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType(context: "DocumentManagement", type: typeof(LocalFile))
            )
            : Ok(value: new MetadataContainer(type: typeof(LocalFile), isEntity: true, hasEndpoint: true));
    }

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<LocalFile> queryOptions) =>
        Ok(value: Service.GetAll());

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            LocalFile result = Service.Get(fileId: key);
            return result is null ? NotFound() : Ok(value: result);
        }
        catch (System.Security.SecurityException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Post([FromBody] LocalFile entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await Service.AddFileAsync(newFile: entity));
    }

    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] LocalFile entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: ModelState);
        }

        entity.Id = key;
        return Ok(value: await Service.UpdateFileAsync(updatedFile: entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<LocalFile> delta)
    {
        LocalFile originalEntity = Service.Get(fileId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await Service.UpdateFileAsync(updatedFile: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await Service.DeleteAsync(fileId: key);
        return Ok();
    }
}