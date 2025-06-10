using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Middleware.Models;

namespace Middleware.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/User
    [HttpGet("getall")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/User/{id}
    [HttpGet("get/{id}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return user;
    }

    // POST: api/User
    [HttpPost("create")]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = new User
        {
            Id = dto.Id,
            Name = dto.Name,
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // PUT: api/User/{id}
    [HttpPut("update/name/{id}")]
    public async Task<IActionResult> UpdateUser(string id, string name)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        user.Name = name;
        user.LastLogin = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Users.Any(e => e.Id == id))
                return NotFound();
            throw;
        }
        //return timezone info
        return Ok(new
        {
            LastLogin = user.LastLogin.ToString("yyyy-MM-ddTHH:mm:ssZ") // ISO 8601 format
        });
    }

    // DELETE: api/User/{id}
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Log the deletion event (optional)
        return Ok(new
        {
            Message = "User deleted successfully",
            UserId = id
        });
    }
}
