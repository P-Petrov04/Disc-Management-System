using API.ViewModel;
using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

    [HttpGet]
    public IActionResult GetUser()
    {
        return Ok(new { Message = "List of users will be returned here" });
    }

    [HttpPost("create")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult CreateUser([FromBody] CreateUserModel model)
    {
        if(model == null)
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
}
