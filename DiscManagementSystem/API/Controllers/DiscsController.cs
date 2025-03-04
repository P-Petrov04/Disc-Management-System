﻿using API.ViewModel;
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
            photoUrl = $"/images/{uniqueFileName}";
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
    public IActionResult GetDiscs(int page = 1, int size = 5, string? title = null, string? artist = null)
    {
        if (page < 1 || size < 1)
        {
            return BadRequest("Page and size must be positive numbers.");
        }

        // Validate search parameters
        if (!string.IsNullOrEmpty(title) && title.Length > 100)
        {
            return BadRequest("Title search parameter is too long.");
        }

        if (!string.IsNullOrEmpty(artist) && artist.Length > 100)
        {
            return BadRequest("Artist search parameter is too long.");
        }

        var query = _discRepository.GetAll().AsQueryable();

        // Search by disc name
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(d => d.Title != null && d.Title.ToLower().Contains(title.ToLower()));
        }

        // Search by artist name
        if (!string.IsNullOrEmpty(artist))
        {
            query = query.Where(d => d.Artist != null && d.Artist.ToLower().Contains(artist.ToLower()));
        }

        var totalDiscs = query.Count();

        // Return error if no discs are found
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
