using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using System.ComponentModel.DataAnnotations;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly HttpClient _httpClient;

        public UsersController(UserDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Update fields
            user.Name = request.Name;
            user.Email = request.Email;
            user.Role = request.Role;

            // Only update password if a new one is provided
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            // Update DateOfBirth if provided
            if (request.DateOfBirth.HasValue)
            {
                user.DateOfBirth = request.DateOfBirth;
            }

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // Sync to AuthService
                await SyncToAuthService(user, "update");

                return Ok(new
                {
                    message = "User updated successfully",
                    user = new
                    {
                        user.Id,
                        user.Name,
                        user.Email,
                        user.Role,
                        user.DateOfBirth
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Update failed: {ex.Message}" });
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    return BadRequest(new { message = "Name is required" });
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    return BadRequest(new { message = "Password is required" });
                }

                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Hash the password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                // Add and save
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ User created successfully: {user.Name} ({user.Email})");

                // ✅ NOW SYNC TO AUTHSERVICE
                await SyncToAuthService(user, "create");

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    user.DateOfBirth
                });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ Database Error: {dbEx.Message}");
                Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                return StatusCode(500, new { message = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = $"Failed to create user: {ex.Message}" });
            }
        }


        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Sync deletion to AuthService
            await SyncToAuthService(user, "delete");

            return NoContent();
        }

        // POST: api/users/sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncFromAuthService([FromBody] SyncRequest request)
        {
            if (request.Operation == "create")
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser == null)
                {
                    var user = new User
                    {
                        AuthId = request.AuthId,
                        Name = request.Name,
                        Email = request.Email,
                        Password = request.Password,
                        Role = request.Role,
                        DateOfBirth = request.DateOfBirth
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
            }
            else if (request.Operation == "update")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user != null)
                {
                    user.Name = request.Name;
                    user.Role = request.Role;
                    user.DateOfBirth = request.DateOfBirth;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { message = "Sync completed" });
        }

        // Helper method to sync to AuthService
        private async Task SyncToAuthService(User user, string operation)
        {
            try
            {
                // ✅ MAKE SURE THIS URL IS CORRECT
                var authServiceUrl = "http://localhost:5001/api/auth/sync";

                var syncData = new
                {
                    operation = operation,
                    userId = user.Id,
                    name = user.Name,
                    email = user.Email,
                    password = user.Password, // Already hashed
                    role = user.Role,
                    dateOfBirth = user.DateOfBirth
                };

                Console.WriteLine($"🔄 Syncing to AuthService: {operation} - {user.Email}");

                var response = await _httpClient.PostAsJsonAsync(authServiceUrl, syncData);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Sync successful to AuthService");
                }
                else
                {
                    Console.WriteLine($"❌ Sync failed: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error details: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error syncing to AuthService: {ex.Message}");
            }
        }
    }


        // DTO for Update User Request
        public class UpdateUserRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Password is optional for updates
        public string? Password { get; set; }

        [Required]
        public string Role { get; set; } = "Student";

        public DateTime? DateOfBirth { get; set; }
    }

    // DTO for Sync Request
    public class SyncRequest
    {
        public string Operation { get; set; } = string.Empty;
        public int AuthId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
