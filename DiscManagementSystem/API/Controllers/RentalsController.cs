using API.ViewModel;
using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RentalsController : ControllerBase
{
    private readonly BaseRepository<Disc> _discRepository;
    private readonly BaseRepository<User> _userRepository;
    private readonly BaseRepository<Rental> _rentalRepository;

    public RentalsController(BaseRepository<Disc> discRepository, BaseRepository<User> userRepository, BaseRepository<Rental> rentalRepository)
    {
        _discRepository = discRepository;
        _userRepository = userRepository;
        _rentalRepository = rentalRepository;
    }

    /// <summary>
    /// Create a new rental
    /// </summary>
    [HttpPost("create")]
    public IActionResult CreateRental([FromBody] CreateRentalModel model)
    {
        if (model == null)
        {
            return BadRequest("Rental data is required.");
        }

        Rental newRental = new Rental()
        {
            UserId = model.UserId,
            DiscId = model.DiscId,
            RentalDate = model.RentalDate,
            RentalDuration = model.RentalDuration,
            Status = model.Status,
            IsRentalExceeded = false // Default value
        };

        _rentalRepository.Add(newRental);
        return Ok(new { Message = "Rental created successfully.", Rental = newRental });
    }

    /// <summary>
    /// Get all rentals with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public IActionResult GetRentals(int page = 1, int size = 5, int? userId = null, int? discId = null, string? status = null)
    {
        if (page < 1 || size < 1)
        {
            return BadRequest("Page and size must be positive numbers.");
        }

        var query = _rentalRepository.GetAll().AsQueryable();

        // Filter by UserId
        if (userId.HasValue)
        {
            query = query.Where(r => r.UserId == userId);
        }

        // Filter by DiscId
        if (discId.HasValue)
        {
            query = query.Where(r => r.DiscId == discId);
        }

        // Filter by Status
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status != null && r.Status.ToLower() == status.ToLower());
        }

        var totalRentals = query.Count();

        // Return error if no rentals are found
        if (totalRentals == 0)
        {
            return NotFound(new { Message = "No rentals found matching your search criteria." });
        }

        var rentals = query.Skip((page - 1) * size).Take(size).ToList();

        return Ok(new
        {
            TotalRentals = totalRentals,
            Page = page,
            PageSize = size,
            Data = rentals
        });
    }

    [HttpGet("my-rentals")]
    [Authorize] // Ensure the user is authenticated
    public IActionResult GetMyRentals(int page = 1, int size = 5)
    {
        // Get the UserId from the token
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { Message = "Invalid user token." });
        }

        // Filter rentals by the logged-in user's UserId
        var query = _rentalRepository.GetAll().Where(r => r.UserId == userId);

        var totalRentals = query.Count();

        // Paginate the results
        var rentals = query.Skip((page - 1) * size).Take(size).ToList();

        return Ok(new
        {
            TotalRentals = totalRentals,
            Page = page,
            PageSize = size,
            Data = rentals
        });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRental(int id, [FromBody] UpdateRentalModel model)
    {
        var rental = _rentalRepository.FirstOrDefault(r => r.RentalId == id);
        if (rental == null)
        {
            return NotFound(new { Message = "Rental not found." });
        }

        if (model.ReturnDate.HasValue) rental.ReturnDate = model.ReturnDate.Value;
        if (!string.IsNullOrEmpty(model.Status)) rental.Status = model.Status;
        if (model.IsRentalExceeded.HasValue) rental.IsRentalExceeded = model.IsRentalExceeded.Value;

        _rentalRepository.Update(rental);
        return Ok(new { Message = "Rental updated successfully.", Rental = rental });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteRental(int id)
    {
        var rental = _rentalRepository.FirstOrDefault(r => r.RentalId == id);
        if (rental == null)
        {
            return NotFound(new { Message = "Rental not found." });
        }

        _rentalRepository.Delete(rental);
        return Ok(new { Message = "Rental deleted successfully." });
    }
}
