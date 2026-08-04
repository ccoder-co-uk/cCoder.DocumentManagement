// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Extensions.OData;
using cCoder.DocumentManagement.Dependencies;
using cCoder.DocumentManagement.Models.OData;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Models.Exceptions;
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
    IFolderManager service
) : ODataController
{

    [HttpPost]
    [ActionName("Copy")]
    public async Task<IActionResult> PostCopyAsync(
        string source,
        string destination,
        int sourceAppId,
        int destAppId
    )
    {
        try
        {
            List<Result<Guid?>> copiedFolders = await service.CopyAsync(
                source: source,
                destination: destination,
                sourceAppId: sourceAppId,
                destAppId: destAppId);

            return Ok(value: copiedFolders);
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

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query[key: "extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(value: ODataConventionModelBuilderExtensions.CreateIEdmModel()
                    .GetExtendedMetadataForType(context: "DocumentManagement", type: typeof(Folder)))
                : Ok(value: MetadataContainerDependency.CreateMetadataContainer(type: typeof(Folder), isEntity: true, hasEndpoint: true));
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
    public IActionResult GetAll(ODataQueryOptions<Folder> queryOptions)
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
            Folder result = service.Get(folderId: key);

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
    public async Task<IActionResult> Post([FromBody] Folder newFolder)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: ModelState);
            }

            Folder addedFolder = await service.AddFolderAsync(newFolder: newFolder);

            return StatusCode(statusCode: StatusCodes.Status201Created, value: addedFolder);
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Folder updatedFolder)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: ModelState);
            }

            updatedFolder.Id = key;

            return Ok(value: await service.UpdateFolderAsync(updatedFolder: updatedFolder));
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
    public async Task<IActionResult> PutPatchAsync([FromRoute] Guid key, Delta<Folder> updatedFolderDelta)
    {
        try
        {
            Folder originalEntity = service.Get(folderId: key);

            if (originalEntity == null)
            {
                return NotFound();
            }

            updatedFolderDelta.Patch(original: originalEntity);

            return Ok(value: await service.UpdateFolderAsync(updatedFolder: originalEntity));
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
            await service.DeleteAsync(folderId: key);

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