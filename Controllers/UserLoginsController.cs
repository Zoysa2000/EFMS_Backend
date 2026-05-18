using Microsoft.AspNetCore.Mvc;
using EmployeeAdminPortal.Model;
using EmployeeAdminPortal.Data;

namespace EmployeeAdminPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLoginController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserLoginController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null ||
                string.IsNullOrWhiteSpace(loginDto.Email) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { success = false, message = "Email and password are required." });
            }

            try
            {
                bool validUser = _userRepository.ValidateUserCredentials(loginDto.Email, loginDto.Password);

                if (validUser)
                {
                    return Ok(new { success = true, message = "Login successful." });
                }
                else
                {
                    return Unauthorized(new { success = false, message = "Invalid email or password." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Server error: " + ex.Message });
            }
        }
    }
}

