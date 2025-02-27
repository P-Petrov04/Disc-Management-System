using API.ViewModel;
using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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

  
    /// <returns>Confirmation message and created disc</returns>
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


    /// <returns>List of discs with pagination metadata</returns>
    [HttpGet]
    public IActionResult GetDiscs(int page = 1, int size = 5, string? title = null, string? artist = null)
    {



        if (page < 1 || size < 1)
        {
            return BadRequest("Page and size must be positive numbers.");
        }

        var query = _discRepository.GetAll().AsQueryable();
        //search by disc name
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(u => u.Title != null && u.Title.ToLower().Contains(title.ToLower()));
        }

        //search by artist name
        if (!string.IsNullOrEmpty(artist))
        {
            query = query.Where(u => u.Artist != null && u.Artist.ToLower().Contains(artist.ToLower()));
        }


        var totalDiscs = query.Count();
      //return error if disc is not found or doesnt exsist
        if (totalDiscs == 0)
        {
            return NotFound(new { Message = "No discs found matching your search criteria." });
        }

        var discs = query.Skip((page - 1) * size).Take(size).ToList();

        return Ok(new
        {
            TotalDiscs = totalDiscs,
            Page = page,
            PageSize = size,
            Data = discs
        });
    }
}
