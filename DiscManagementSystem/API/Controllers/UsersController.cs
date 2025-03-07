using API.ViewModel;
using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly BaseRepository<User> _userRepository;

    public UsersController(BaseRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("create")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult CreateUser([FromBody] CreateUserModel model)
    {
        if (model == null)
        {
            return BadRequest("User data is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate input
        if (string.IsNullOrEmpty(model.FirstName))
        {
            return BadRequest("First name is required.");
        }

        if (string.IsNullOrEmpty(model.LastName))
        {
            return BadRequest("Last name is required.");
        }

        if (string.IsNullOrEmpty(model.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            return BadRequest("Password is required.");
        }

        // Check if the email is already registered
        var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Email == model.Email);
        if (existingUser != null)
        {
            return Conflict(new { Message = "Email is already registered." });
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = model.Password,
            PhoneNumber = model.PhoneNumber,
            RegistrationDate = DateTime.UtcNow,
            IsActive = true
        };

        _userRepository.Add(user);
        return Ok(new { Message = "User created successfully.", User = user });
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetUsers(int? pageP = null, int? sizeP = null, string? email = null)
    {
        if (pageP.HasValue && sizeP.HasValue)
        {
            if (pageP < 1 || sizeP < 1)
            {
                return BadRequest("Page and size must be positive numbers.");
            }
        }

        // Validate search parameter
        if (!string.IsNullOrEmpty(email) && email.Length > 100)
        {
            return BadRequest("Email search parameter is too long.");
        }

        var query = _userRepository.GetAll().AsQueryable();

        // Search by email
        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => u.Email != null && u.Email.ToLower().Contains(email.ToLower()));
        }

        var totalUsers = query.Count();

        // Return error if no users are found
        if (totalUsers == 0)
        {
            return NotFound(new { Message = "No users found matching your search criteria." });
        }

        List<User> users;
        if (pageP.HasValue && sizeP.HasValue) 
        {
            users = query
            .Skip((pageP.Value - 1) * sizeP.Value)
            .Take(sizeP.Value)
            .ToList();
        }
        else
        {
            users = query.ToList();
        }
        

        return Ok(new
        {
            TotalUsers = totalUsers,
            Page = pageP ?? 1,
            Size = sizeP ?? totalUsers,
            Users = users
        });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserModel model)
    {
        if (model == null)
        {
            return BadRequest("User data is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _userRepository.FirstOrDefault(u => u.UserId == id);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        // Validate input
        if (!string.IsNullOrEmpty(model.FirstName) && model.FirstName.Length <= 100) user.FirstName = model.FirstName;
        if (!string.IsNullOrEmpty(model.LastName) && model.LastName.Length <= 100) user.LastName = model.LastName;

        if (!string.IsNullOrEmpty(model.Email))
        {
            user.Email = model.Email;
        }

        if (!string.IsNullOrEmpty(model.Password))
        {
            user.Password = model.Password;
        }

        if (!string.IsNullOrEmpty(model.PhoneNumber))
        {
            user.PhoneNumber = model.PhoneNumber;
        }

        _userRepository.Update(user);
        return Ok(new { Message = "User updated successfully.", User = user });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userRepository.FirstOrDefault(u => u.UserId == id);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        _userRepository.Delete(user);
        return Ok(new { Message = "User deleted successfully." });
    }

}
