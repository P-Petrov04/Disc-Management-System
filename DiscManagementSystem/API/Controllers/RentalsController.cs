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
    private readonly BaseRepository<Rental> _rentalRepository;
    private readonly BaseRepository<User> _userRepository;
    private readonly BaseRepository<Disc> _discRepository;

    public RentalsController(BaseRepository<Rental> rentalRepository, BaseRepository<User> userRepository, BaseRepository<Disc> discRepository)
    {
        _rentalRepository = rentalRepository;
        _userRepository = userRepository;
        _discRepository = discRepository;
    }

    [HttpPost("create")]
    [Authorize]
    public IActionResult CreateRental([FromBody] CreateRentalModel model)
    {
        if (model == null)
        {
            return BadRequest("Rental data is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate input
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (model.DiscId <= 0)
        {
            return BadRequest("Invalid DiscId.");
        }

        if (model.RentalDate > DateTime.Now)
        {
            return BadRequest("Rental date cannot be in the future.");
        }

        if (model.RentalDuration <= 0)
        {
            return BadRequest("Rental duration must be a positive number.");
        }

        if (string.IsNullOrEmpty(model.Status) || (model.Status != "Active" && model.Status != "Returned"))
        {
            return BadRequest("Status must be either 'Active' or 'Returned'.");
        }

        if (!_discRepository.GetAll().Select(d => d.DiscId).Contains(model.DiscId))
        {
            return BadRequest("Disc with this id is not found.");
        }
        
        var currDisc = _discRepository.FirstOrDefault(d => d.DiscId == model.DiscId);
        if (!currDisc.IsAvailable)
        {
            return BadRequest("This Disc is not available.");
        }


        Rental newRental = new Rental()
        {
            UserId = int.Parse(userIdClaim.Value),
            DiscId = model.DiscId,
            RentalDate = model.RentalDate,
            RentalDuration = model.RentalDuration,
            Status = model.Status,
            IsRentalExceeded = false // Default value
        };

        currDisc.IsAvailable = false;
        _rentalRepository.Add(newRental);
        return Ok(new { Message = "Rental created successfully.", Rental = model });
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetRentals(int page = 1, int size = 5, int? userId = null, int? discId = null, string? status = null)
    {
        if (page < 1 || size < 1)
        {
            return BadRequest("Page and size must be positive numbers.");
        }

        // Validate search parameters
        if (userId.HasValue && userId <= 0)
        {
            return BadRequest("Invalid UserId.");
        }

        if (discId.HasValue && discId <= 0)
        {
            return BadRequest("Invalid DiscId.");
        }

        if (!string.IsNullOrEmpty(status) && status != "Active" && status != "Returned")
        {
            return BadRequest("Status must be either 'Active' or 'Returned'.");
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
        if (page < 1 || size < 1)
        {
            return BadRequest("Page and size must be positive numbers.");
        }

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
        if (model == null)
        {
            return BadRequest("Rental data is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var rental = _rentalRepository.FirstOrDefault(r => r.RentalId == id);
        if (rental == null)
        {
            return NotFound(new { Message = "Rental not found." });
        }

        // Validate input
        if (model.ReturnDate.HasValue && model.ReturnDate.Value > DateTime.Now)
        {
            return BadRequest("Return date cannot be in the future.");
        }

        if (model.ReturnDate.HasValue && model.ReturnDate.Value < rental.RentalDate)
        {
            return BadRequest("Return date cannot be in the future.");
        }

        if (!string.IsNullOrEmpty(model.Status) && model.Status != "Active" && model.Status != "Returned")
        {
            return BadRequest("Status must be either 'Active' or 'Returned'.");
        }

        if (model.ReturnDate.HasValue) rental.ReturnDate = model.ReturnDate.Value;
        if (!string.IsNullOrEmpty(model.Status)) rental.Status = model.Status;
        if (model.IsRentalExceeded.HasValue) rental.IsRentalExceeded = model.IsRentalExceeded.Value;

        var currDisc = _discRepository.FirstOrDefault(d => d.DiscId == rental.DiscId);
        if(model.Status == "Active")
        {
            currDisc.IsAvailable = false;
        }
        else if (model.Status == "Returned")
        {
            currDisc.IsAvailable = true;
        }

        _rentalRepository.Update(rental);
        return Ok(new { Message = "Rental updated successfully.", Rental = model });
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
