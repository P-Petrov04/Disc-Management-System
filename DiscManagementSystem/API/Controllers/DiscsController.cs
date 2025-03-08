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


    [HttpPost("create")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateDisc([FromForm] CreateDiscModel model)
    {
        if (model == null)
        {
            return BadRequest("Disc data is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate input
        if (string.IsNullOrEmpty(model.Title))
        {
            return BadRequest("Title is required.");
        }

        if (string.IsNullOrEmpty(model.Artist))
        {
            return BadRequest("Artist is required.");
        }

        if (string.IsNullOrEmpty(model.Format))
        {
            return BadRequest("Format is required.");
        }

        if (model.DurationMinutes <= 0)
        {
            return BadRequest("Duration must be a positive number.");
        }

        if (model.ReleaseDate > DateTime.Now)
        {
            return BadRequest("Release date cannot be in the future.");
        }

        string photoUrl = null;
        if (model.Photo != null && model.Photo.Length > 0)
        {
            // Validate file size (e.g., 5MB limit)
            if (model.Photo.Length > 5 * 1024 * 1024) // 5MB in bytes
            {
                return BadRequest("File size must be less than 5MB.");
            }

            // Validate file type (e.g., only allow images)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.Photo.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Only JPG, JPEG, PNG, and GIF files are allowed.");
            }

            // Save the file to a folder (e.g., wwwroot/images)
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name to avoid conflicts
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Photo.CopyToAsync(stream);
            }

            // Store the file URL
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            photoUrl = $"{baseUrl}/images/{uniqueFileName}";
        }

        Disc newDisc = new Disc()
        {
            Title = model.Title,
            Artist = model.Artist,
            ReleaseDate = model.ReleaseDate,
            Format = model.Format,
            DurationMinutes = model.DurationMinutes,
            IsAvailable = true, // Default value
            PhotoUrl = photoUrl
        };

        _discRepository.Add(newDisc);
        return Ok(new { Message = "Disc created successfully.", Disc = newDisc });
    }

    [HttpGet]
    [HttpGet]
    [HttpGet]
    public IActionResult GetDiscs(int? pageP = null, int? sizeP = null, string? title = null, string? artist = null)
    {
        if (pageP.HasValue && sizeP.HasValue)
        {
            if (pageP < 1 || sizeP < 1)
            {
                return BadRequest("Page and size must be positive numbers.");
            }
        }

        var query = _discRepository.GetAll().AsQueryable();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(d => d.Title != null && d.Title.ToLower().Contains(title.ToLower()));
        }

        if (!string.IsNullOrEmpty(artist))
        {
            query = query.Where(d => d.Artist != null && d.Artist.ToLower().Contains(artist.ToLower()));
        }

        var totalDiscs = query.Count();

        if (totalDiscs == 0)
        {
            return NotFound(new { Message = "No discs found matching your search criteria." });
        }

        List<Disc> discs;
        if (pageP.HasValue && sizeP.HasValue)
        {
            discs = query
                .Skip((pageP.Value - 1) * sizeP.Value)
                .Take(sizeP.Value)
                .Select(d => new Disc
                {
                    DiscId = d.DiscId,
                    Title = d.Title,
                    Artist = d.Artist,
                    ReleaseDate = d.ReleaseDate,
                    Format = d.Format,
                    IsAvailable = d.IsAvailable,
                    DurationMinutes = d.DurationMinutes,
                    PhotoUrl = d.PhotoUrl != null
                        ? $"{Request.Scheme}://{Request.Host}{d.PhotoUrl}" // ✅ Fix URL here
                        : null
                })
                .ToList();
        }
        else
        {
            discs = query
                .Select(d => new Disc
                {
                    DiscId = d.DiscId,
                    Title = d.Title,
                    Artist = d.Artist,
                    ReleaseDate = d.ReleaseDate,
                    Format = d.Format,
                    IsAvailable = d.IsAvailable,
                    DurationMinutes = d.DurationMinutes,
                    PhotoUrl = d.PhotoUrl != null
                        ? $"{Request.Scheme}://{Request.Host}{d.PhotoUrl}" // ✅ Fix URL here
                        : null
                })
                .ToList();
        }

        return Ok(new
        {
            TotalDiscs = totalDiscs,
            Page = pageP ?? 1,
            PageSize = sizeP ?? totalDiscs,
            Data = discs
        });
    }


    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateDisc(int id, [FromForm] UpdateDiscModel model)
    {
        if (model == null)
        {
            return BadRequest("Disc data is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var disc = _discRepository.FirstOrDefault(d => d.DiscId == id);
        if (disc == null)
        {
            return NotFound(new { Message = "Disc not found." });
        }

        // Validate input
        if (!string.IsNullOrEmpty(model.Title) && model.Title.Length <= 100) disc.Title = model.Title;
        if (!string.IsNullOrEmpty(model.Artist) && model.Artist.Length <= 100) disc.Artist = model.Artist;
        if (model.ReleaseDate.HasValue)
        {
            if (model.ReleaseDate.Value > DateTime.Now)
            {
                return BadRequest("Release date cannot be in the future.");
            }
            disc.ReleaseDate = model.ReleaseDate.Value;
        }
        if (!string.IsNullOrEmpty(model.Format)) disc.Format = model.Format;
        if (model.DurationMinutes.HasValue)
        {
            if (model.DurationMinutes.Value <= 0)
            {
                return BadRequest("Duration must be a positive number.");
            }
            disc.DurationMinutes = model.DurationMinutes.Value;
        }

        if (model.Photo != null && model.Photo.Length > 0)
        {
            // Validate file size (e.g., 5MB limit)
            if (model.Photo.Length > 5 * 1024 * 1024) // 5MB in bytes
            {
                return BadRequest("File size must be less than 5MB.");
            }

            // Validate file type (e.g., only allow images)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.Photo.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Only JPG, JPEG, PNG, and GIF files are allowed.");
            }

            // Save the file to a folder (e.g., wwwroot/images)
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name to avoid conflicts
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Photo.CopyToAsync(stream);
            }

            // Store the file URL
            disc.PhotoUrl = $"/images/{uniqueFileName}";
        }

        _discRepository.Update(disc);
        return Ok(new { Message = "Disc updated successfully.", Disc = disc });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteDisc(int id)
    {
        var disc = _discRepository.FirstOrDefault(d => d.DiscId == id);
        if (disc == null)
        {
            return NotFound(new { Message = "Disc not found." });
        }

        _discRepository.Delete(disc);
        return Ok(new { Message = "Disc deleted successfully." });
    }
}
