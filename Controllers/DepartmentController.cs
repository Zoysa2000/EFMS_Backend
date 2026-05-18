using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentRepository _repository;

        public DepartmentController(IConfiguration configuration)
        {
            _repository = new DepartmentRepository(configuration);
        }

        
        [HttpGet]
        public ActionResult<IEnumerable<Department>> GetDepartments()
        {
            var depts = _repository.GetAllDept();
            return Ok(depts);
        }

    }
}
