using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADAPIService;
using ADAPIService.implementations;
using ADAPICommon.model;
using ADAPIService.interfaces;

namespace ActiveDirectoryApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController : Controller
{
    protected IServiceInterface _service;
    public BaseController(IServiceInterface service)
    {
        
    }
}





