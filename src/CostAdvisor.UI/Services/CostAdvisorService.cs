using CostAdvisor.Shared.Models;
using System.Net.Http.Json;

namespace CostAdvisor.UI.Services
{
    public class CostAdvisorService: ICostAdvisorService
    {
        private readonly HttpClient _http;

        public CostAdvisorService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<NormalizedCost>> GetResourceCostsAsync(DateTime from, DateTime to)
        {
            var url = $"api/billing/costs?from={from:o}&to={to:o}";
            return await _http.GetFromJsonAsync<List<NormalizedCost>>(url) ?? new List<NormalizedCost>();
        }

        public async Task<List<Recommendation>> GetRecommendationsAsync(DateTime from, DateTime to)
        {
            var url = $"api/billing/recommendations?from={from:o}&to={to:o}";
            return await _http.GetFromJsonAsync<List<Recommendation>>(url) ?? new List<Recommendation>();
        }
        public async Task FetchCostsAsync()
        {
            var from = DateTime.UtcNow.AddDays(-30);
            var to = DateTime.UtcNow;
            var request = new FetchRequest { AccountId = string.Empty, From = from, To = to };
            var content = JsonContent.Create(request);

            var response = await _http.PostAsync("/api/billing/fetch", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task<DashboardSummary?> GetDashboardAsync(DateTime from,DateTime to)
        {
            var url = $"api/billing/dashboard?from={from:o}&to={to:o}";
            return await _http.GetFromJsonAsync<DashboardSummary>(url) ?? new DashboardSummary();
        }
    }
}
