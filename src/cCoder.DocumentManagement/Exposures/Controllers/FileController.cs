// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Extensions.OData;
using cCoder.DocumentManagement.Models.OData;
using cCoder.DocumentManagement.Models.Exceptions;
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

public partial class FileController(
    IFileManager service
) : ODataController
{

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query[key: "extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(
                    value: ODataConventionModelBuilderExtensions.CreateIEdmModel()
                        .GetExtendedMetadataForType(context: "DocumentManagement", type: typeof(LocalFile)))
                : Ok(value: new MetadataContainer(type: typeof(LocalFile), isEntity: true, hasEndpoint: true));
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
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
    public IActionResult GetAll(ODataQueryOptions<LocalFile> queryOptions)
    {
        try
        {
            return Ok(value: service.GetAll());
        }
        catch (System.Security.SecurityException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

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
            LocalFile result = service.Get(fileId: key);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(value: result);
        }
        catch (System.Security.SecurityException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
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
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: ModelState);
            }

            LocalFile addedFile = await service.AddFileAsync(newFile: entity);

            return StatusCode(statusCode: StatusCodes.Status201Created, value: addedFile);
        }
        catch (DocumentManagementValidationException)
        {
            return BadRequest();
        }
        catch (System.Security.SecurityException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
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
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: ModelState);
            }

            entity.Id = key;

            return Ok(value: await service.UpdateFileAsync(updatedFile: entity));
        }
        catch (DocumentManagementValidationException)
        {
            return BadRequest();
        }
        catch (System.Security.SecurityException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [AcceptVerbs("PATCH", "MERGE")]
    [ActionName("Patch")]
    public async Task<IActionResult> PutPatchAsync([FromRoute] Guid key, Delta<LocalFile> updatedFileDelta)
    {
        try
        {
            LocalFile originalEntity = service.Get(fileId: key);

            if (originalEntity == null)
            {
                return NotFound();
            }

            updatedFileDelta.Patch(original: originalEntity);

            return Ok(value: await service.UpdateFileAsync(updatedFile: originalEntity));
        }
        catch (DocumentManagementValidationException)
        {
            return BadRequest();
        }
        catch (System.Security.SecurityException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        try
        {
            await service.DeleteAsync(fileId: key);

            return NoContent();
        }
        catch (DocumentManagementValidationException)
        {
            return BadRequest();
        }
        catch (System.Security.SecurityException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}