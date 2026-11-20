// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.DocumentManagement.Brokers.Loggings;
using cCoder.DocumentManagement.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using LocalFileContent = cCoder.Data.Models.DMS.FileContent;


namespace cCoder.DocumentManagement.Exposures.Controllers;

public partial class FileContentController(
    IFileContentManager service,
    ILoggingBroker loggingBroker
) : ODataController, ICompositionExposure
{

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
            IQueryable<LocalFileContent> result = service.GetAll();
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
            LocalFileContent result = service.GetAll()
                .FirstOrDefault(predicate: fileContent => fileContent.Id == key);

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
    public async Task<IActionResult> Post([FromBody] LocalFileContent entity)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(error: "The request is invalid.");
            }

            LocalFileContent addedFileContent = await service.AddFileContentAsync(newFileContent: entity);
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return StatusCode(statusCode: StatusCodes.Status201Created, value: addedFileContent);
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] LocalFileContent entity)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(error: "The request is invalid.");
            }

            entity.Id = key;

            LocalFileContent result = await service.UpdateFileContentAsync(updatedFileContent: entity);
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
            await service.DeleteAsync(fileContentId: key);
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