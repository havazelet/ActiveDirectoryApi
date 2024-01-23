using ADAPICommon.model;
using ADAPIService.implementations;
using ADAPIService.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActiveDirectoryApi.Controllers;

public class OUController : ADController
{
    public OUController(IServiceInterface service, ILogger<ADController> logger) : base(service, logger)
    {

    } 

    [HttpPost("create")]
    public IActionResult CreateGroup(ADObject adObject)
    {;
        _service.CreateADObject(adObject, "OU");
        return Ok();
    }
}
