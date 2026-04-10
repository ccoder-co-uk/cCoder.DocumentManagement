using cCoder.DocumentManagement.Api.OData;
using cCoder.DocumentManagement.Models;
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

public partial class FolderController : ODataController
{
    protected IFolderOrchestrationService Service { get; }

    public FolderController(IFolderOrchestrationService service, ILogger<FolderController> log)
    {
        Service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CopyAsync(
        string source,
        string destination,
        int sourceAppId,
        int destAppId
    ) => Ok(await Service.CopyAsync(source, destination, sourceAppId, destAppId));

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                new cCoder.DocumentManagement.Api.OData.DocumentManagementModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType("Core", typeof(Folder))
            )
            : Ok(new MetadataContainer(typeof(Folder), true, true));
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
    public IActionResult GetAll(ODataQueryOptions<Folder> queryOptions) => Ok(Service.GetAll());

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
            IQueryable<Folder> result = Service.GetAll().Where(folder => folder.Id == key);
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
    public async Task<IActionResult> Post([FromBody] Folder entity)
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Folder entity)
    {
        if (!ModelState.IsValid)
            return new cCoder.DocumentManagement.Api.OData.BadRequestResult(ModelState);

        entity.Id = key;
        return Ok(await Service.UpdateAsync(entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<Folder> delta)
    {
        Folder originalEntity = Service.Get(key);
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

















