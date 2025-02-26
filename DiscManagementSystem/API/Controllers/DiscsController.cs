using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class DiscsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDiscs()
    {
        return Ok(new { Message = "List of discs will be returned here" });
    }

    [HttpPost]
    public IActionResult CreateDisc([FromBody] object disc)
    {
        return Ok(new { Message = "Disc created successfully" });
    }
}
