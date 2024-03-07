using ADAPICommon.model;
using ADAPIService.implementations;
using ADAPIService.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActiveDirectoryApi.Controllers;

public class GroupController : ADController
{
    public GroupController(ILogger<ADController> logger, IServiceInterface service) : base(logger, service)
    {

    }

    [HttpPost("create")]
    public IActionResult CreateGroup(ADObject adObject)
    {
        _service.CreateADObject(adObject, "group");
        return Ok();
    }

    [HttpPost("modify")]
    public IActionResult ModifyGroup(ModifyModel newAdObject)
    {
        _service.ModifyADObject(newAdObject, "group");
        return Ok();
    }
}
