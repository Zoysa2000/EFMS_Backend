using Microsoft.AspNetCore.Mvc;
using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Model;  // Import the model

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPasswordChange : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserPasswordChange(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] PasswordChangeRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Email and new password must be provided.");
            }

            bool success = _userRepository.ChangePassword(request.Email, request.NewPassword);

            if (success)
            {
                return Ok(new { message = "Password updated successfully." });
            }
            else
            {
                return NotFound(new { message = "User with the specified email not found." });
            }
        }
    }
}

