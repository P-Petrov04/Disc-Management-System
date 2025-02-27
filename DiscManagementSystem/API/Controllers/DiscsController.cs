using API.ViewModel;
using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class DiscsController : ControllerBase
{
    private readonly BaseRepository<Disc> _discRepository;

    public DiscsController(BaseRepository<Disc> discRepository)
    {
        _discRepository = discRepository;
    }

    [HttpGet]
    public IActionResult GetDiscs()
    {
        return Ok(new { Message = "List of discs will be returned here" });
    }

    [HttpPost("create")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult CreateDisc([FromBody] CreateDiscModel model)
    {
        if (model == null)
        {
            return BadRequest("Disc data is required.");
        }

        Disc newDisc = new Disc()
        {
            Title = model.Title,
            Artist = model.Artist,
            ReleaseDate = model.ReleaseDate,
            Format = model.Format,
            DurationMinutes = model.DurationMinutes
        };

        _discRepository.Add(newDisc);
        return Ok(new { Message = "Disc created successfully.", Disc = newDisc });
    }
}
