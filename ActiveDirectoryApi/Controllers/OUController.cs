using ADAPICommon.model;
using ADAPIService.implementations;
using ADAPIService.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActiveDirectoryApi.Controllers;

public class OUController : BaseController
{
    public OUController(IServiceInterface service) : base(service)
    {

    } 

    [HttpPost("create")]
    public IActionResult CreateGroup(ADObject adObject)
    {;
        _service.CreateADObject(adObject, "OU");
        return Ok();
    }
}
