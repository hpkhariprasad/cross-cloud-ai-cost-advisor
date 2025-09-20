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

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(DateTime from, DateTime to)
        {
            var costs = await _billingService.GetCostsAsync(from, to);
            return Ok(costs);
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> Fetch([FromBody] FetchRequest request)
        {
            await _billingService.FetchAndStoreAsync(request.AccountId, request.From, request.To);
            return Ok(new { message = "Costs fetched and stored" });
        }
    }
}
