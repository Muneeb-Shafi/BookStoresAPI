using BCrypt.Net;
using BookStore.Models;
using BookStoresAPI.DTOs;
using BookStoresAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        
        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost ("register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var user = new User
            {
                Username = userDTO.username,
                Email = userDTO.email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.password),
                Role = "Admin"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new {message = "User Creation Success"});
        }


        [HttpPost ("Login")]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            try
            {
                if(userDTO == null)
                {
                    return BadRequest("Invalid");
                }

                var user  = _context.Users.FirstOrDefault(x => x.Email == userDTO.email);

                if (!BCrypt.Net.BCrypt.Verify(userDTO.password, user.Password))
                {
                    return BadRequest("Invalid Password");
                }

                var tokenString = Generate(user);
                return Ok(new {Token = tokenString});
            }
            catch (Exception ex) { 
                return StatusCode(500, ex.Message);
            }
        }

        private string Generate(User user)
        {
            try
            {

                var key = _config["Jwt:Key"];
                var issuer = _config["Jwt:Issuer"];
                var audience = _config["Jwt:Audience"];
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    throw new InvalidOperationException("JWT configuration is missing or invalid.");
                }

                Console.WriteLine($"Key: {key}");
                Console.WriteLine($"Issuer: {issuer}");
                Console.WriteLine($"Audience: {audience}");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credentials
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                string tokenString = tokenHandler.WriteToken(token);
                return tokenString;
            }
            catch (TypeInitializationException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
;
        }


        [HttpPost ("Invite")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Invite(UserDTO userDto)
        {
            var user = new User
            {
                Username = userDto.username,
                Email = userDto.email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.password),
                Role = "Employee"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Employee Added successfully" });
        }


    }
}
