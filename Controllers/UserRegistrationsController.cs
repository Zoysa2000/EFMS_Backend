using Microsoft.AspNetCore.Mvc;
using EmployeeAdminPortal.Model;
using EmployeeAdminPortal.Validators;
using EmployeeAdminPortal.Data;
using Oracle.ManagedDataAccess.Client;

namespace EmployeeAdminPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRegistrationsController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserRegistrationsController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto userDto)
        {
            var (isValid, message) = UserRegistrationValidator.Validate(userDto);
            if (!isValid)
            {
                return BadRequest(new { message });
            }

           try
            {
                _userRepository.AddUser(userDto);
            }
           

            catch (OracleException ex) when (ex.Number == 1) // ORA-00001
            {
                return Conflict(new { message = "This e-mail is already registered." });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = $"Unexpected error: {ex.Message}" });
            }

            return Ok(new { message = "User registered successfully!" });
        }

    }
}






