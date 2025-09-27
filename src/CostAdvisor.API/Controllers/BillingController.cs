using CostAdvisor.Core.Services;
using CostAdvisor.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CostAdvisor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly BillingService _billingService;

        public BillingController(BillingService billingService)
        {
            _billingService = billingService;
        }


        [HttpPost("fetch")]
        public async Task<IActionResult> Fetch([FromBody] FetchRequest request)
        {
            await _billingService.FetchAndStoreAsync(request.AccountId, request.From, request.To);
            return Ok(new { message = "Costs fetched and stored" });
        }

        [HttpGet("costs")]
        public async Task<IActionResult> GetCosts(DateTime from, DateTime to)
        {
            if (from > to)
            {
                return BadRequest("Invalid date range.");
            }
            try
            {
                var costs = await _billingService.GetCostsAsync(from, to);
                return Ok(costs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(DateTime from,DateTime to)
        {
            var summary = await _billingService.GetDashboardSummaryAsync(from,to);
            return Ok(summary);
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations(DateTime from, DateTime to)
        {
            var costs = await _billingService.GetCostsAsync(from, to);
            var recs = costs.SelectMany(c => c.Recommendations).ToList();
            return Ok(recs);
        }

    }
}
