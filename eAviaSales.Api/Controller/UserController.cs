using eAviaSales.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private static readonly List<User> Users = [];
    private static int _nextId = 1;

    [HttpGet("all")]
    public IActionResult GetAllUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found" });
        }

        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] User user)
    {
        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        Users.Add(user);
        return Created($"/api/user/{user.Id}", user);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        var existingUser = Users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found" });
        }

        existingUser.UserName = updatedUser.UserName;
        existingUser.Email = updatedUser.Email;

        return Ok(existingUser);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found" });
        }

        Users.Remove(user);
        return NoContent();
    }
}
