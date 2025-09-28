using CostAdvisor.Core.Repositories;
using CostAdvisor.Shared.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace CostAdvisor.Infrastructure.Repositories
{
    public class RecommendationService : IRecommendationService
    {
        private readonly OpenAIClient _openAI;
        private readonly IConfiguration _config;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(OpenAIClient openAI, IConfiguration config, ILogger<RecommendationService> logger)
        {
            _openAI = openAI;
            _config = config;
            _logger = logger;
        }

        public async Task<IDictionary<int, List<Recommendation>>> GenerateRecommendationsAsync(IEnumerable<NormalizedCost> costs)
        {

            try
            {
                var model = _config["OpenAI:Model"] ?? "gpt-4o-mini";
                var costSummary = string.Join("\n", costs.Select(c =>
                    $"{c.Account} {c.Service} in {c.Region}: ${c.UsageAmount}"));

                var chatClient = _openAI.GetChatClient(model);

                var sb = new StringBuilder();
                foreach (var c in costs)
                {
                    sb.AppendLine($"Id={c.Id} | Provider={c.AccountId} | Service={c.Service} | Region={c.Region} | Usage={c.UsageAmount} | Cost={c.UsageAmount}");
                }

                var systemMessage = new SystemChatMessage("You are a cloud cost optimization assistant.");
                var userMessage = new UserChatMessage($@"
Here is a list of cloud costs:

{sb}

For each line (by Id), generate up to 2 recommendations. 
Format one recommendation per line exactly like this:
Id=<CostId> | Message | Confidence (0-100) | EstimatedSavings (numeric) | Service
");
                var messages = new List<ChatMessage> { systemMessage, userMessage };

                ChatCompletion completion = await chatClient.CompleteChatAsync(messages);
                var aiText = completion.Content?.FirstOrDefault()?.Text ?? string.Empty;

                return ParseBatch(aiText);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falling back to fake AI recommendations");
                return GenerateDummyBatch(costs);
            }

        }
        private IDictionary<int, List<Recommendation>> ParseBatch(string aiText)
        {
            var dict = new Dictionary<int, List<Recommendation>>();

            var lines = Regex.Split(aiText, @"\r?\n")
                .Where(l => !string.IsNullOrWhiteSpace(l));

            foreach (var line in lines)
            {
                // Example: "Id=12 | Right-size VMs | 85 | 120"
                var parts = line.Split('|');
                if (parts.Length < 5) continue;

                if (!int.TryParse(parts[0].Replace("Id=", "").Trim(), out var costId))
                    continue;

                var message = parts[1].Trim();
                var confidence = decimal.TryParse(parts[2].Trim().Replace("%", ""), out var c) ? c : 0m;
                var savings = decimal.TryParse(parts[3].Trim(), out var s) ? s : 0m;
                var service = parts[4].Trim();

                var rec = new Recommendation
                {
                    CostId = costId,
                    Message = message,
                    Confidence = confidence,
                    EstimatedSavings = savings,
                    CreatedOn = DateTime.UtcNow,
                    Category = service
                };

                if (!dict.ContainsKey(costId))
                    dict[costId] = new List<Recommendation>();

                dict[costId].Add(rec);
            }

            return dict;
        }
        private IDictionary<int, List<Recommendation>> GenerateDummyBatch(IEnumerable<NormalizedCost> costs)
        {
            return costs.ToDictionary(
                c => c.Id,
                c => new List<Recommendation>
                {
                new Recommendation { CostId = c.Id,Category=c.Service, Message = "Fake rec: move to reserved instances", Confidence = 75, EstimatedSavings = 50, CreatedOn = DateTime.UtcNow },
                new Recommendation { CostId = c.Id,Category=c.Service, Message = "Fake rec: reduce storage tier", Confidence = 60, EstimatedSavings = 20, CreatedOn = DateTime.UtcNow }
                });
        }
    }

}
