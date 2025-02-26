using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RentalsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRentals()
    {
        return Ok(new { Message = "List of rentals will be returned here" });
    }

    [HttpPost]
    public IActionResult CreateRentals([FromBody] object disc)
    {
        return Ok(new { Message = "rental created successfully" });
    }
}
