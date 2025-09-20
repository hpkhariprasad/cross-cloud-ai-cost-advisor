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

        public async Task<List<NormalizedCost>> GetResourceCostsAsync()
        {
            var from = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.ToString("yyyy-MM-dd");

            return await _http.GetFromJsonAsync<List<NormalizedCost>>(
                $"/api/billing/summary?from={from}&to={to}");
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


    }
}
