using VaderSharp2;
using Spring2026_Project3_smthomas12.ViewModels;

namespace Spring2026_Project3_smthomas12.Services
{
    public static class SentimentHelper
    {
        private static readonly SentimentIntensityAnalyzer Analyzer = new();

        public static SentimentResult Analyze(string text)
        {
            var scores = Analyzer.PolarityScores(text);
            return new SentimentResult
            {
                Text = text,
                Compound = Math.Round(scores.Compound, 3),
                Label = GetLabel(scores.Compound),
                CssClass = GetCssClass(scores.Compound)
            };
        }

        public static List<SentimentResult> AnalyzeAll(List<string> texts)
            => texts.Select(Analyze).ToList();

        public static (double avg, string label, string css) Average(List<SentimentResult> results)
        {
            if (results.Count == 0) return (0, "Neutral", "sentiment-neutral");
            var avg = Math.Round(results.Average(r => r.Compound), 3);
            return (avg, GetLabel(avg), GetCssClass(avg));
        }

        private static string GetLabel(double compound) =>
            compound >= 0.05 ? "Positive" :
            compound <= -0.05 ? "Negative" : "Neutral";

        private static string GetCssClass(double compound) =>
            compound >= 0.05 ? "sentiment-positive" :
            compound <= -0.05 ? "sentiment-negative" : "sentiment-neutral";
    }
}