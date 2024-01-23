using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADAPIService;
using ADAPIService.implementations;
using ADAPICommon.model;
using ADAPIService.interfaces;

namespace ActiveDirectoryApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class ADController : Controller
{
    protected IServiceInterface _service;
    public ADController(IServiceInterface service)
    {
        
    }
}





