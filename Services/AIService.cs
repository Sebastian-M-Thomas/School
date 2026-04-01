using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Spring2026_Project3_smthomas12.Services
{
    public class AIService
    {
        private readonly ChatClient _chatClient;

        public AIService(IConfiguration configuration)
        {
            var apiKey = configuration["AzureOpenAI:ApiKey"]!;
            var endpoint = configuration["AzureOpenAI:Endpoint"]!;
            var deployment = configuration["AzureOpenAI:DeploymentName"]!;

            var client = new AzureOpenAIClient(
                new Uri(endpoint),
                new System.ClientModel.ApiKeyCredential(apiKey)
            );

            _chatClient = client.GetChatClient(deployment);
        }

        public async Task<List<string>> GetMovieReviewsAsync(string movieTitle, int year)
        {
            var prompt = $"""
                Generate exactly 5 short movie reviews for the film "{movieTitle}" ({year}).
                Each review should be 2-3 sentences long and reflect a different perspective
                (e.g. action fan, critic, casual viewer, film student, family viewer).
                Format your response as exactly 5 lines, each starting with a number and period:
                1. [review]
                2. [review]
                3. [review]
                4. [review]
                5. [review]
                Do not include any other text, headers, or blank lines.
                """;

            var completion = await _chatClient.CompleteChatAsync(
                new UserChatMessage(prompt)
            );

            var raw = completion.Value.Content[0].Text;
            return ParseNumberedList(raw, 5);
        }

        public async Task<List<string>> GetActorTweetsAsync(string actorName)
        {
            var prompt = $"""
                Generate exactly 10 short tweets (280 characters or fewer each) that fans
                might post on Twitter/X about the actor {actorName}.
                Include a mix of positive, neutral, and negative sentiments.
                Format your response as exactly 10 lines, each starting with a number and period:
                1. [tweet]
                2. [tweet]
                3. [tweet]
                4. [tweet]
                5. [tweet]
                6. [tweet]
                7. [tweet]
                8. [tweet]
                9. [tweet]
                10. [tweet]
                Do not include any other text, headers, or blank lines.
                """;

            var completion = await _chatClient.CompleteChatAsync(
                new UserChatMessage(prompt)
            );

            var raw = completion.Value.Content[0].Text;
            return ParseNumberedList(raw, 10);
        }

        private static List<string> ParseNumberedList(string raw, int expected)
        {
            var results = new List<string>();

            var lines = raw
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 0);

            foreach (var line in lines)
            {
                var dotIndex = line.IndexOf('.');
                if (dotIndex > 0 && dotIndex < 4 && int.TryParse(line[..dotIndex], out _))
                    results.Add(line[(dotIndex + 1)..].Trim());
                else
                    results.Add(line);

                if (results.Count == expected) break;
            }

            while (results.Count < expected)
                results.Add("(Unavailable)");

            return results;
        }
    }
}