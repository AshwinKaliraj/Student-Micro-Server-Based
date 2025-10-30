using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AuthController(AuthDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { success = false, message = "Email and password are required" });
                }

                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new { success = false, message = "User already exists" });
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new AuthUser
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    Role = request.Role ?? "Student",
                    
                    DateOfBirth = request.DateOfBirth ?? DateTime.Now,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ✅ Sync to UserService
                await SyncToUserService(user);

                return Ok(new
                {
                    success = true,
                    message = "User registered successfully",
                    data = new
                    {
                        userId = user.Id,
                        email = user.Email,
                        name = user.Name,
                        role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized(new { success = false, message = "Invalid credentials" });
                }

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    data = new
                    {
                        token = token,
                        email = user.Email,
                        role = user.Role,
                        userId = user.Id,
                        name = user.Name
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: api/auth/sync (from UserService)
        [HttpPost("sync")]
        public async Task<IActionResult> SyncFromUserService([FromBody] SyncRequest request)
        {
            try
            {
                if (request.Operation == "create")
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                    if (existingUser == null)
                    {
                        var user = new AuthUser
                        {
                            Name = request.Name,
                            Email = request.Email,
                            PasswordHash = request.Password,
                            Role = request.Role,
                           
                            DateOfBirth = request.DateOfBirth,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.Users.Add(user);
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
                    }
                }
                else if (request.Operation == "delete")
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                    if (user != null)
                    {
                        _context.Users.Remove(user);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Sync successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        // ✅ Sync helper method
        private async Task SyncToUserService(AuthUser user)
        {
            try
            {
                var userServiceUrl = "http://localhost:7080/api/users/sync";
                var userData = new
                {
                    operation = "create",
                    authId = user.Id,
                    name = user.Name,
                    email = user.Email,
                    password = user.PasswordHash,
                    role = user.Role,
                  
                    dateOfBirth = user.DateOfBirth
                };

                var response = await _httpClient.PostAsJsonAsync(userServiceUrl, userData);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to sync to UserService: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing to UserService: {ex.Message}");
            }
        }

        private string GenerateJwtToken(AuthUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey not found");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Issuer not found");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Audience not found");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("name", user.Name)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
         // ✅ Added
        public DateTime? DateOfBirth { get; set; }    // ✅ Added
    }

    public class SyncRequest
    {
        public string Operation { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        // ✅ Added
        public DateTime DateOfBirth { get; set; }     // ✅ Added
    }
}
