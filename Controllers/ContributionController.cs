using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Model;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionController : ControllerBase
    {
        private readonly ContributionRepository _repository;

        public ContributionController(IConfiguration configuration)
        {
            _repository = new ContributionRepository(configuration);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] List<Contribution> contributions)
        {
            if (contributions == null || contributions.Count == 0)
                return BadRequest("No data received.");

            try
            {
                await _repository.SaveContributionsAsync(contributions);
                return Ok(new { message = "Contributions uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Log exception if logging is configured
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

