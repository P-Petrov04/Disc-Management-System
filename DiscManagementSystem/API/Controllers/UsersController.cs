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



    /// <summary>
    /// Update an existing user (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserModel model)
    {
        var user = _userRepository.FirstOrDefault(d => d.UserId == id);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        if (!string.IsNullOrEmpty(model.FirstName)) user.FirstName = model.FirstName;
        if (!string.IsNullOrEmpty(model.LastName)) user.LastName = model.LastName;
        if (!string.IsNullOrEmpty(model.Email)) user.Email = model.Email;
        if (!string.IsNullOrEmpty(model.Password)) user.Password = model.Password;
        if (!string.IsNullOrEmpty(model.PhoneNumber)) user.PhoneNumber = model.PhoneNumber;


        _userRepository.Update(user);
        return Ok(new { Message = "user updated successfully.", user = user });
    }




    /// <summary>
    /// Delete a user (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userRepository.FirstOrDefault(d => d.UserId == id);
        if (user == null)
        {
            return NotFound(new { Message = "user not found." });
        }

        _userRepository.Delete(user);
        return Ok(new { Message = "user deleted successfully." });
    }

}
