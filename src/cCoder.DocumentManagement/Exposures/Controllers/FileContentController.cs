// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Extensions.OData;
using cCoder.DocumentManagement.Dependencies;
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
using LocalFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.DocumentManagement.Exposures.Controllers;

public partial class FileContentController(
    IFileContentManager service
) : ODataController
{

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query[key: "extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(value: ODataConventionModelBuilderExtensions.CreateIEdmModel()
                    .GetExtendedMetadataForType(context: "DocumentManagement", type: typeof(LocalFileContent)))
                : Ok(value: MetadataContainerDependency.CreateMetadataContainer(type: typeof(LocalFileContent), isEntity: true, hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<LocalFileContent> queryOptions)
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
            LocalFileContent result = service.GetAll()
                .FirstOrDefault(predicate: fileContent => fileContent.Id == key);

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
    public async Task<IActionResult> Post([FromBody] LocalFileContent entity)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: ModelState);
            }

            LocalFileContent addedFileContent = await service.AddFileContentAsync(newFileContent: entity);

            return StatusCode(statusCode: StatusCodes.Status201Created, value: addedFileContent);
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] LocalFileContent entity)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: ModelState);
            }

            entity.Id = key;

            return Ok(value: await service.UpdateFileContentAsync(updatedFileContent: entity));
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
    public async Task<IActionResult> PutPatchAsync(
        [FromRoute] Guid key,
        Delta<LocalFileContent> updatedFileContentDelta)
    {
        try
        {
            LocalFileContent originalEntity = service.Get(fileContentId: key);

            if (originalEntity == null)
            {
                return NotFound();
            }

            updatedFileContentDelta.Patch(original: originalEntity);

            return Ok(value: await service.UpdateFileContentAsync(updatedFileContent: originalEntity));
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
            await service.DeleteAsync(fileContentId: key);

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