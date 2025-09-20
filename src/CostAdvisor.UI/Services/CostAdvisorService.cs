using CostAdvisor.Shared.Models;

namespace CostAdvisor.UI.Services
{
    public class CostAdvisorService: ICostAdvisorService
    {
        private readonly HttpClient _http;

        public CostAdvisorService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<NormalizedCost>> GetResourceCostsAsync()
        {
            // Example date range: last 30 days
            var from = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.ToString("yyyy-MM-dd");

            return await _http.GetFromJsonAsync<List<NormalizedCost>>(
                $"/api/billing/summary?from={from}&to={to}");
        }
        public async Task FetchCostsAsync()
        {
            var response = await _http.PostAsync("/api/billing/fetch", null);
            response.EnsureSuccessStatusCode();
        }


    }
}
