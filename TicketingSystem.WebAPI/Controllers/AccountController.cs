using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedDTOs;
using SharedDTOs.Enum;
using Swashbuckle.AspNetCore.Annotations;
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


        [HttpPost("Login")]
        [SwaggerOperation(
            Summary = "Login to your account.",
            Description = "Validates credentials and generates a Bearer token required for all protected endpoints.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Login successfully, token returned")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid username or password")]
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


        [HttpPost("Register")]
        [SwaggerOperation(
            Summary = "Register a new account,.",
            Description = "Create new account in the system.")]
        [SwaggerResponse(StatusCodes.Status200OK, "User registered successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Username already exists or passwords do not match")]
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

        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Log out the current user",
            Description = "Sets the user status to Inactive. Requires a valid JWT token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "User logged out successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "User session not found")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
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

            return NotFound("User not found.");
        }

        [HttpGet("GetAccountInfo")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get profile info for the logged-in user",
            Description = "Returns the full user profile based on the identity provided in the JWT token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Profile info retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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

        [HttpDelete]
        [Authorize]
        [SwaggerOperation(
            Summary = "Delete a user.",
            Description = "Delete a user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "User deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest ,"Could not delete user.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        public async Task<IActionResult> Delete(string username)
        {
            _logger.LogInformation("DELETE called to delete a user ");

            var user = await _userManager.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result =await _userManager.Delete(username);
            if (result)
            {
                _logger.LogInformation("User with Username: {username} deleted succssfully ", username);
                return Ok("User deleted successfully.");
            }
            else
            {
                _logger.LogInformation("Deleting user with Username: {username} Failed ", username);
                return BadRequest("Could not delete user.");
            }
        }

        [HttpGet("Staff")]
        [Authorize] 
        [SwaggerOperation(
            Summary = "Get all staff members",
            Description = "Returns a list of users with Staff roles for ticket assignment.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Staff list retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No staff members found")]
        public async Task<IActionResult> GetStaff()
        {
            var staff = await _userManager.GetStaff();

            if (staff == null || !staff.Any())
            {
                _logger.LogWarning("No staff members found in the system.");
                return NotFound("No staff found");
            }

            return Ok(staff);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddStaff")]
        [SwaggerOperation(
            Summary = "Admin: Create a new staff account.",
            Description = "Generates a temporary password and sets user status to Pending. Returns the temporary password.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Staff created successfully. Returns the temporary password string.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Username already exists.")]
        public async Task<IActionResult> AddStaff([FromBody] NewUserDto user)
        {
            _logger.LogInformation("Admin is creating a new staff member: {Username}", user.Username);

            var exist = await _userManager.GetUserByUsername(user.Username);
            if (exist != null)
            {
                return BadRequest("Username already exists.");
            }

            string tempPassword = await _userManager.AddStaff(user);

            _logger.LogInformation("Staff {Username} created with temporary password.", user.Username);

            return Ok(new
            {
                message = "Staff account created successfully.",
                temporaryPassword = tempPassword
            });
        }

        [Authorize]
        [HttpPut("UpdateProfile")]
        [SwaggerOperation(
                    Summary = "Update current user profile",
                    Description = "Updates the first name, last name, and email for the authenticated user. Identified by the JWT token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Profile updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data provided or update failed")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {

            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("UpdateProfile failed: No username found.");
                return Unauthorized("User session not found.");
            }

            _logger.LogInformation("PUT called to update profile for user: {Username}", username);

            var result = await _userManager.UpdateProfile(username, model);

            if (result)
            {
                _logger.LogInformation("Profile for {Username} updated successfully.", username);
                return Ok("Profile updated successfully.");
            }

            _logger.LogWarning("Profile update for {Username} failed in the Business Logic layer.", username);
            return BadRequest("Failed to update profile.");
        }

        [Authorize]
        [HttpPatch("ChangePassword")]
        [SwaggerOperation(
            Summary = "Change password for user.",
            Description = "Allows an active/pending user to change their password by providing the current one.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Password changed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Current password incorrect or user not active.")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _userManager.ChangePassword(username, model);

            if (success)
            {
                _logger.LogInformation("User {Username} changed their password.", username);
                return Ok("Password updated successfully.");
            }

            return BadRequest("Could not change password. Please check your current password.");
        }


        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "List all registered users.",
            Description = "Returns a paginated list of all users. restricted to administrative roles.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List of users retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
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
