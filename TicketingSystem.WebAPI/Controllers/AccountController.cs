using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedDTOs;
using SharedDTOs.Enum;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketingSystem.Services.Interfaces;

namespace TicketingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userManager, IConfiguration config, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            configuration = config;
            _logger = logger;
        }

        /// <summary>
        /// Login to your account.
        /// </summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            _logger.LogInformation("POST called to login");

            var result = await _userManager.Validatelogin(login);

            if(!result)
            {
                _logger.LogWarning("login failed - Invalid username or password");
                return Unauthorized("Invalid username or password");
            }

            var u = await _userManager.GetUserByUsername(login.Username);

            var user = new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                Status = UserStatus.Active
            };

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.Username),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               claims: claims,
               issuer: configuration["JWT:Issuer"],
               audience: configuration["JWT:Audience"],
               expires: DateTime.UtcNow.AddHours(1),
               signingCredentials: creds
           );

            _logger.LogInformation("Token generated for {Username}", login.Username);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                username = u.Username
            });
        }

        /// <summary>
        /// Register - Sign Up.
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Create(NewUserDto user)
        {
            _logger.LogInformation("POST called to register a new user");
            var exist = await _userManager.GetUserByUsername(user.Username);

            if (exist != null)
            {
                _logger.LogWarning("Failed - Username already exist");
                return BadRequest("Username already exists.");
            }

            if(user.Password != user.ConfirmPassword)
            {
                _logger.LogWarning("Failed - Passwords does not match.");
                return BadRequest("Confirm password must match your password !");
            }

            await _userManager.Add(user);
            _logger.LogInformation("User registered successfully");
            return Ok("User registered successfully");
        }

        /// <summary>
        /// Log out of your account.
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return BadRequest();

            var user = await _userManager.GetUserByUsername(username);
            if (user != null)
            {
                var success =await _userManager.SetUserInActive(username);

                if (success)
                {
                    _logger.LogInformation("User {Username} successfully loged out.", username);
                    return Ok("User loged out successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to logout user {Username}.", username);
                    return StatusCode(500, "Failed logout user.");
                }
            }

            return BadRequest("User not found.");
        }

        ///<summary>
        ///Get users info.
        ///</summary>
        [HttpGet("GetAccountInfo")]
        [Authorize]
        public async Task<IActionResult> GetAccountInfo()
        {

            var username = User.Identity?.Name;
            _logger.LogInformation("Get called to get account info for user {Username}", username);

            var user = await _userManager.GetUserByUsername(username);

            if (user == null)
            {
                _logger.LogWarning("User Not Found");
                return NotFound("User Not Found");
            }

            _logger.LogInformation("Successfully get info for user {Username}", username);
            return Ok(user);
        }


        /// <summary>
        /// Get Users.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Read(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "username",
            [FromQuery] string sortDirection = "asc")
        {
            var (users, totalCount) = await _userManager.GetUsersPaged(page, pageSize, search, sortBy, sortDirection);

            return Ok(new
            {
                items = users,
                totalCount = totalCount
            });
        }
    }
}
