using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using STOCK.API.Core.IRepository;
using STOCK.API.Helpers.Params;

namespace STOCK.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepo dashboardRepo;

        public DashboardController(IDashboardRepo dashboardRepo)
        {
            this.dashboardRepo = dashboardRepo;
        }

        [HttpGet("barchart")]
        public async Task<IActionResult> GetBarChartData([FromQuery] DashboardParams dashboardParams)
        {
            var data = await dashboardRepo.GetBarChartData(dashboardParams);
            return Ok(data);
        }
    }
}