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
    private readonly ILogger<ADController> _logger;
    protected IServiceInterface _service;
    public ADController(ILogger<ADController> logger, IServiceInterface service)
    {
        _service = service;
        _logger = logger;

    }
}





