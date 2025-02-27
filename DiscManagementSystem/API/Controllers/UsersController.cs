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
public class UsersController : ControllerBase
{
    private readonly BaseRepository<User> _userRepository;

    public UsersController(BaseRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
  
    /// <returns>Confirmation message and created user</returns>
    [HttpPost("create")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult CreateUser([FromBody] CreateUserModel model)
    {
        if (model == null)
        {
            return BadRequest("User data is required.");
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

    /// <returns>List of users with pagination metadata</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult GetUsers(int page = 1, int size = 10, string? email = null)
    {
        var query = _userRepository.GetAll().AsQueryable();

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => u.Email != null && u.Email.ToLower().Contains(email.ToLower()));
        }

        var totalUsers = query.Count();
        var users = query
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return Ok(new
        {
            TotalUsers = totalUsers,
            Page = page,
            Size = size,
            Users = users
        });
    }

}
