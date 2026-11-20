// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Brokers.Loggings;
using cCoder.Data.Models.Security;
using cCoder.DocumentManagement.Models.Exceptions;
using cCoder.DocumentManagement.Models.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.DocumentManagement.Exposures.Controllers;

public class FolderRoleController(
    IFolderRoleManager service,
    ILoggingBroker loggingBroker
) : ODataController, ICompositionExposure
{

    [HttpGet]
    [EnableQuery(AllowedArithmeticOperators = AllowedArithmeticOperators.All, AllowedFunctions = AllowedFunctions.AllFunctions, AllowedLogicalOperators = AllowedLogicalOperators.All, AllowedQueryOptions = AllowedQueryOptions.All, MaxAnyAllExpressionDepth = 3, MaxExpansionDepth = 3)]
    [ActionName("Get")]
    public IActionResult GetAll()
    {
        try
        {
            IQueryable<FolderRole> result = service.GetAll();
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return Ok(value: result);
        }
        catch (DocumentManagementValidationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The document management request is invalid.");
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (UnauthorizedAccessException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
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
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(error: "The request is invalid.");
            }

            FolderRole addedFolderRole = await service.AddFolderRoleAsync(newFolderRole: newFolderRole);
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return StatusCode(statusCode: StatusCodes.Status201Created, value: addedFolderRole);
        }
        catch (DocumentManagementServiceException exception) when (exception.GetBaseException() is DuplicateFolderRoleException)
        {
            loggingBroker.LogWarning(
                exception: exception,
                message: "Controller request conflicted with existing state.");

            return Conflict(error: new { Message = "The folder is already in the selected role." });
        }
        catch (DocumentManagementValidationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The document management request is invalid.");
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (UnauthorizedAccessException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
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
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(error: "The request is invalid.");
            }

            await service.DeleteAllFolderRoleAsync(deletedFolderRole: deletedFolderRole.Value);
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return Ok();
        }
        catch (DocumentManagementValidationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The document management request is invalid.");
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (UnauthorizedAccessException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
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

            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return NoContent();
        }
        catch (DocumentManagementServiceException exception) when (exception.GetBaseException() is System.Security.SecurityException)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
        }
        catch (DocumentManagementValidationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The document management request is invalid.");
        }
        catch (ArgumentException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (UnauthorizedAccessException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(exception: exception, message: "Controller authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden, value: "Authorization failed.");
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Controller request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}