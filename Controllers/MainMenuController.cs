using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Model;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainMenuController : ControllerBase
    {
        private readonly MainMenuRepository _repository;

        public MainMenuController(IConfiguration configuration)
        {
            _repository = new MainMenuRepository(configuration);
        }

        [HttpGet]
        public ActionResult<IEnumerable<MainMenu>> GetMenus()
        {
            var menus = _repository.GetMenus();
            return Ok(menus);
        }
    }
}

