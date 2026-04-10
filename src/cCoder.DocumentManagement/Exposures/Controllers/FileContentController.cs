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

public partial class FileContentController : ODataController
{
    protected IFileContentOrchestrationService Service { get; }

    public FileContentController(
        IFileContentOrchestrationService service,
        ILogger<FileContentController> log
    )
    {
        Service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                new cCoder.DocumentManagement.Api.OData.DocumentManagementModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType("Core", typeof(LocalFileContent))
            )
            : Ok(new MetadataContainer(typeof(LocalFileContent), true, true));
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
    public IActionResult GetAll(ODataQueryOptions<LocalFileContent> queryOptions) => Ok(Service.GetAll());

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
            IQueryable<LocalFileContent> result = Service.GetAll().Where(fileContent => fileContent.Id == key);
            return Ok(SingleResult.Create(result));
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
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(ModelState);

        return Ok(await Service.AddAsync(entity));
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
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(ModelState);

        entity.Id = key;
        return Ok(await Service.UpdateAsync(entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<LocalFileContent> delta)
    {
        LocalFileContent originalEntity = Service.Get(key);
        if (originalEntity == null)
            return NotFound();

        delta.Patch(originalEntity);
        return Ok(await Service.UpdateAsync(originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await Service.DeleteAsync(key);
        return Ok();
    }
}
















