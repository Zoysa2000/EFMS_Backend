using EmployeeAdminPortal.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountController : ControllerBase
    {
        private readonly CountRepository _repository;

        public CountController(IConfiguration configuration)
        {
            _repository = new CountRepository(configuration);
        }
        [HttpGet("employeeCount")]
        public IActionResult GetEmployeeCount()
        {
            try
            {
                int count = _repository.GetEmployeeCount();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee count", error = ex.Message });
            }
        }
        [HttpGet("employeeCountIT")]
        public IActionResult GetEmployeeCountIT()
        {
            try
            {
                int count = _repository.GetEmployeeCountIT();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee count", error = ex.Message });
            }
        }
        [HttpGet("employeeCountHR")]
        public IActionResult GetEmployeeCountHR()
        {
            try
            {
                int count = _repository.GetEmployeeCountHR();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee count", error = ex.Message });
            }
        }
        [HttpGet("employeeCountFinance")]
        public IActionResult GetEmployeeCountFinance()
        {
            try
            {
                int count = _repository.GetEmployeeCountFinance();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee count", error = ex.Message });
            }
        }
        [HttpGet("employeeCountSales")]
        public IActionResult GetEmployeeCountSales()
        {
            try
            {
                int count = _repository.GetEmployeeCountSales();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee count", error = ex.Message });
            }
        }

    }
}
