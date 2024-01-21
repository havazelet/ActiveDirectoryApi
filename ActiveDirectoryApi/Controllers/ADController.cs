using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADAPIService;
using ADAPIService.implementations;
using ADAPICommon.model;

namespace ActiveDirectoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
      
    }

    public class UserController : BaseController
    {
        [HttpPost("user/create")]
        public IActionResult PostOU(ADObject userModel)
        {
            Service adObjectService = new Service();
            adObjectService.CreateADObject(userModel, "user");
            return Ok(); 
        }
    }

    public class GroupController : BaseController
    {
        [HttpPost("group/create")]
        public IActionResult PostUser(ADObject GroupModel)
        {
            Service adObjectService = new Service();
            adObjectService.CreateADObject(GroupModel, "group");
            return Ok(); 
        }

    public class OUController : BaseController
    {
        [HttpPost]
        public IActionResult PostGroup(ADObject OUModel)
        {
            Service adObjectService = new Service();
            adObjectService.CreateADObject(OUModel, "OU");
            return Ok();
        }
    }

}



 }




