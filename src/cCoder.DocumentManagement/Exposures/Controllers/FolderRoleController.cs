// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Extensions.OData;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.DMS;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Services.Orchestrations;
using cCoder.DocumentManagement.Models.Exceptions;
using cCoder.DocumentManagement.Models.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.DocumentManagement.Exposures.Controllers;

public class FolderRoleController(
    IFolderRoleManager service
) : ODataController
{

    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            return Ok(value: ODataMetadataProvider.GetMetadata(
                type: typeof(FolderRole),
                isEntity: true,
                hasEndpoint: true));
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll()
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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FolderRole newFolderRole)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: base.ModelState);
            }

            FolderRole addedFolderRole = await service.AddFolderRoleAsync(newFolderRole: newFolderRole);

            return StatusCode(statusCode: StatusCodes.Status201Created, value: addedFolderRole);
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

    [HttpPost]
    public async Task<IActionResult> DeleteAll([FromBody] ODataCollection<FolderRole> deletedFolderRole)
    {
        try
        {
            if (!base.ModelState.IsValid)
            {
                return new cCoder.DocumentManagement.Models.OData.BadRequestResult(modelState: base.ModelState);
            }

            await service.DeleteAllFolderRoleAsync(deletedFolderRole: deletedFolderRole.Value);

            return Ok();
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