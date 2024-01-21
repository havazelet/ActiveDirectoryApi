using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ADAPIService;
using ADAPIService.implementations;

namespace ActiveDirectoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADController : ControllerBase
    {
        [HttpPost("{actionType}/read")]
        public ActionResult<string> Post(object OIIdentifier, object attributes, object identifier, string actionType)
        {
            if (OIIdentifier == null || attributes == null || identifier == null)
            {
                return NotFound("Resource not found");
            }
            else
            {
                User user = new User();

                if (actionType == "users")
                {
                    user.GetUser(OIIdentifier, attributes, identifier);
                    return Ok("User added");
                }
                else if (actionType == "ou")
                {
                    return Ok("OU added");
                }
                else
                {
                    return BadRequest("Invalid action type");
                }
            }


        }


    }
}
