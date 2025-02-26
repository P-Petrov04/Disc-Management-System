using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUser()
    {
        return Ok(new { Message = "List of users will be returned here" });
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] object disc)
    {
        return Ok(new { Message = "user created successfully" });
    }
}
