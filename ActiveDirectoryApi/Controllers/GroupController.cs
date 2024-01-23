using ADAPICommon.model;
using ADAPIService.implementations;
using ADAPIService.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActiveDirectoryApi.Controllers;

public class GroupController : ADController
{
    public GroupController(IServiceInterface service, ILogger<ADController> logger) : base(service, logger)
    {

    }

    [HttpPost("create")]
    public IActionResult CreateGroup(ADObject adObject)
    {
        _service.CreateADObject(adObject, "group");
        return Ok();
    }
}
