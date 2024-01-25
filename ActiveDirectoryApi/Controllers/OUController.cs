using ADAPICommon.model;
using ADAPIService.implementations;
using ADAPIService.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActiveDirectoryApi.Controllers;

public class OUController : ADController
{
    public OUController(ILogger<ADController> logger, IServiceInterface service) : base(logger, service)
    {

    } 

    [HttpPost("create")]
    public IActionResult CreateGroup(ADObject adObject)
    {;
        _service.CreateADObject(adObject, "OU");
        return Ok();
    }
}
