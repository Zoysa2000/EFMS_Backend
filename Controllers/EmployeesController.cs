using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Model;
using EmployeeAdminPortal.Validators;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeRepository _repository;
        private readonly IWebHostEnvironment _env;

        public EmployeesController(EmployeeRepository repository,
                                   IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

      
        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { message = "No image uploaded." });

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Construct the URL properly
                var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                return Ok(new { imageUrl = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Image upload failed", error = ex.Message });
            }
        }

        [HttpPost("uploadNewImage")]
        public async Task<IActionResult> UploadNewImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { message = "No image uploaded." });

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Construct the URL properly
                var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                return Ok(new { imageUrl = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Image upload failed", error = ex.Message });
            }
        }


        [HttpPost("addEmployee")]
        public IActionResult AddEmployee([FromBody] EmployeeDetails employee)
        {
            var (isValid, message) = EmployeeDetailsValidator.Validate(employee);
            if (!isValid) return BadRequest(new { message });

            if (string.IsNullOrWhiteSpace(employee.ImageUrl))
                return BadRequest(new { message = "ImageUrl is missing (upload the image first)" });

            try
            {
                string empId = _repository.AddEmployee(employee);

                if (string.IsNullOrEmpty(empId))
                    return StatusCode(500, new { message = "Error: Employee was not inserted." });

                return Ok(new { message = "Employee inserted successfully.", empId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error inserting employee.", error = ex.Message });
            }
        }



        [HttpPut("updateEmployee/{id}")]
        public IActionResult UpdateEmployee(
        [FromRoute] string id,
        [FromBody] EmployeeDetails employee)
        {
            employee.EmpId = id;

            var (isValid, msg) = EmployeeDetailsValidator.Validate(employee);
            if (!isValid) return BadRequest(new { msg });

            try
            {
                int rows = _repository.UpdateEmployee(employee);
                if (rows == 0)
                    return NotFound(new { message = $"Employee {id} not found." });

                return Ok(new { message = "Employee updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Update failed.", error = ex.Message });
            }
        }




        [HttpDelete("deleteEmployee/{deptName}/{empId}")]
        public IActionResult DeleteEmployee(
        [FromRoute] string deptName,
        [FromRoute] string empId)
        {
            try
            {
                int rows = _repository.DeleteEmployee(deptName, empId);

                if (rows == 0)
                    return NotFound(new
                    {
                        message = $"Employee {empId} not found in {deptName.Trim()}."
                    });

                return Ok(new
                {
                    message = $"Employee {empId} deleted from {deptName.Trim()}."
                });
            }
            catch (OracleException ox)
            {
               
                return StatusCode(409, new
                {
                    message = "Delete failed – foreign‑key constraint.",
                    error = ox.Message
                });
            }
            catch (ArgumentException ax)
            {
         
                return BadRequest(new
                {
                    message = ax.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Delete failed.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("getEmployee")]
        public ActionResult<IEnumerable<EmployeeDetails>> GetEmployees()
        {
            try
            {
                var employees = _repository.GetAllEmployees();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                
                Console.Error.WriteLine(ex);

                return StatusCode(500, new { message = "Error retrieving employees.", error = ex.ToString() });
            }
        }

        // EmployeesController.cs
        [HttpGet("{id}")]
        public ActionResult<EmployeeDetails> GetEmployeeById(string id)
        {
            try
            {
                var employee = _repository.GetEmployeeById(id);
                if (employee == null) return NotFound(new { message = $"Employee {id} not found." });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee.", error = ex.Message });
            }
        }

        [HttpGet("byDepartment/{deptName}")]
        public ActionResult<IEnumerable<EmployeeDetails>> GetByDepartment(string deptName)
        {
            try
            {
                var employees = _repository.GetEmployeesByDepartment(deptName);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error fetching employees for department {deptName}.", error = ex.Message });
            }
        }


        [HttpGet("{deptName}/{empId}")]
        public IActionResult GetEmployee(string deptName, string empId)
        {
            try
            {
                var emp = _repository.GetEmployeeByEmpIdAndDepartment(empId, deptName);
                return emp is null
                    ? NotFound(new { message = "No matching employee." })
                    : Ok(emp);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        [HttpGet("getEmployeeDetails/{id}")]
        public ActionResult<EmployeeDetails> GetEmployeeDefault(string id)
        {
            try
            {
                var employee = _repository.GetEmployeeById(id);
                if (employee == null) return NotFound(new { message = $"Employee {id} not found." });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee.", error = ex.Message });
            }
        }



    }
}
