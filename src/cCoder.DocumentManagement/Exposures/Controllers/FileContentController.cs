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
using LocalFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.DocumentManagement.Exposures.Controllers;

public partial class FileContentController(
    IFileContentOrchestrationService service
) : ODataController
{

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query[key: "extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                value: new cCoder.DocumentManagement.Api.OData.DocumentManagementModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType(context: "DocumentManagement", type: typeof(LocalFileContent))
            )
            : Ok(value: new MetadataContainer(type: typeof(LocalFileContent), isEntity: true, hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<LocalFileContent> queryOptions) =>
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
            IQueryable<LocalFileContent> result = service.GetAll()
                .Where(predicate: fileContent => fileContent.Id == key);

            return Ok(value: SingleResult.Create(queryable: result));
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
    public async Task<IActionResult> Post([FromBody] LocalFileContent entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: ModelState);
        }

        return Ok(value: await service.AddFileContentAsync(newFileContent: entity));
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] LocalFileContent entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(modelState: ModelState);
        }

        entity.Id = key;
        return Ok(value: await service.UpdateFileContentAsync(updatedFileContent: entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<LocalFileContent> delta)
    {
        LocalFileContent originalEntity = service.Get(fileContentId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await service.UpdateFileContentAsync(updatedFileContent: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(fileContentId: key);
        return Ok();
    }
}