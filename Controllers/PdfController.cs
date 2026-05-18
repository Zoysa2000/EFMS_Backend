using EmployeeAdminPortal.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly PdfRepository _repository;

        public PdfController(IConfiguration configuration)
        {
            _repository = new PdfRepository(configuration);
        }


        [HttpGet("employee/{empId}")]
        public IActionResult GetEmployeeDetails(string empId)
        {
            if (string.IsNullOrWhiteSpace(empId))
                return BadRequest("Employee ID is required.");

            var employee = _repository.GetEmployeeDetailsById(empId);
            if (employee == null)
                return NotFound($"Employee with ID '{empId}' not found.");

            return Ok(employee);
        }

    }
}
