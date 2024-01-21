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
            Service user = new Service();
            user.CreateADObject(userModel);
            return Ok(); 
        }
    }

    public class GroupController : BaseController
    {
        [HttpPost("group/create")]
        public IActionResult PostUser(ADObject GroupModel)
        {


            return Ok(); 
    }

    public class OUController : BaseController
    {
        [HttpPost]
        public IActionResult PostGroup(ADObject OUModel)
        {
           

            return Ok();
        }
    }

}



 }




