// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Api.OData;
using cCoder.DocumentManagement.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.DocumentManagement.Exposures.Controllers;

public partial class FolderController(
    IFolderOrchestrationService service,
    ILogger<FolderController> log
) : ODataController
{

    [HttpPost]
    public async Task<IActionResult> CopyAsync(
        string source,
        string destination,
        int sourceAppId,
        int destAppId
    ) =>
        Ok(value: await service.CopyAsync(source: source, destination: destination, sourceAppId: sourceAppId, destAppId: destAppId));

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query[key: "extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                value: new cCoder.DocumentManagement.Api.OData.DocumentManagementModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType(context: "DocumentManagement", type: typeof(Folder))
            )
            : Ok(value: new MetadataContainer(type: typeof(Folder), isEntity: true, hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<Folder> queryOptions) =>
        Ok(value: service.GetAll());

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
            Folder result = service.Get(folderId: key);
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
    public async Task<IActionResult> Post([FromBody] Folder newFolder)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.AddFolderAsync(newFolder: newFolder));
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Folder entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: ModelState);
        }

        entity.Id = key;
        return Ok(value: await service.UpdateFolderAsync(updatedFolder: entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<Folder> delta)
    {
        Folder originalEntity = service.Get(folderId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await service.UpdateFolderAsync(updatedFolder: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(folderId: key);
        return Ok();
    }
}
