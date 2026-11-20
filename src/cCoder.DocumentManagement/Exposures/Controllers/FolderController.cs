// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Brokers.Loggings;
using cCoder.DocumentManagement.Models;
using cCoder.DocumentManagement.Models.Exceptions;
using cCoder.Data.Models.DMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.DocumentManagement.Exposures.Controllers;

public partial class FolderController(
    IFolderManager service,
    ILoggingBroker loggingBroker
) : ODataController, ICompositionExposure
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

            loggingBroker.LogInformation(message: "Controller request succeeded.");
            return Ok(value: copiedFolders);
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
            IQueryable<Folder> result = service.GetAll();
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
                loggingBroker.LogWarning(message: "Controller resource was not found.");

                return NotFound();
            }

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
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(error: "The request is invalid.");
            }

            Folder addedFolder = await service.AddFolderAsync(newFolder: newFolder);
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return StatusCode(statusCode: StatusCodes.Status201Created, value: addedFolder);
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
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(error: "The request is invalid.");
            }

            updatedFolder.Id = key;

            Folder result = await service.UpdateFolderAsync(updatedFolder: updatedFolder);
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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        try
        {
            await service.DeleteAsync(folderId: key);
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return NoContent();
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