using ADAPICommon.model;
using ADAPIService.implementations;
using ADAPIService.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActiveDirectoryApi.Controllers;

public class UserController : ADController
{

    public UserController(IServiceInterface service, ILogger<ADController> logger) : base(service, logger)
    {

    }

    [HttpPost("create")]
    public IActionResult CreateOU(ADObject adObject)
    {
        _service.CreateADObject(adObject, "user");
        return Ok();
    }
}
