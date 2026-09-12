// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.DocumentManagement.Brokers.Loggings;
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
    IFolderRoleManager service,
    ILoggingBroker loggingBroker
) : ODataController
{

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(value: service.GetAll());
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return Forbid();
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

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
        catch (DocumentManagementServiceException exception) when (exception.GetBaseException() is DuplicateFolderRoleException)
        {
            loggingBroker.LogError(exception: exception, message: "Folder role already exists.");
            return Conflict(error: new { Message = "The folder is already in the selected role." });
        }
        catch (DocumentManagementValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest();
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return Forbid();
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

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
        catch (DocumentManagementValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return BadRequest();
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return Forbid();
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid keyFolderId, [FromRoute] Guid keyRoleId)
    {
        try
        {
            await service.DeleteFolderRoleAsync(deletedFolderRole: new FolderRole
            {
                FolderId = keyFolderId,
                RoleId = keyRoleId,
            });

            return NoContent();
        }
        catch (DocumentManagementServiceException exception) when (exception.GetBaseException() is System.Security.SecurityException)
        {
            loggingBroker.LogError(exception: exception, message: "Folder role deletion denied.");
            return Forbid();
        }
        catch (DocumentManagementValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");
            return BadRequest();
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}