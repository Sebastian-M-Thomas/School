namespace Spring2026_Project3_smthomas12.ViewModels
{
    public class SentimentResult
    {
        public string Text { get; set; } = "";
        public double Compound { get; set; }
        public string Label { get; set; } = "";
        public string CssClass { get; set; } = "";
    }

    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string ImdbLink { get; set; } = "";
        public string Genre { get; set; } = "";
        public int Year { get; set; }
        public byte[] Poster { get; set; } = Array.Empty<byte>();

        public List<string> ActorNames { get; set; } = new();
        public List<SentimentResult> Reviews { get; set; } = new();
        public double AverageSentiment { get; set; }
        public string AverageSentimentLabel { get; set; } = "";
        public string AverageSentimentCss { get; set; } = "";
    }

    public class ActorDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Gender { get; set; } = "";
        public int Age { get; set; }
        public string ImdbLink { get; set; } = "";
        public byte[] Photo { get; set; } = Array.Empty<byte>();

        public List<string> MovieTitles { get; set; } = new();
        public List<SentimentResult> Tweets { get; set; } = new();
        public double AverageSentiment { get; set; }
        public string AverageSentimentLabel { get; set; } = "";
        public string AverageSentimentCss { get; set; } = "";
    }
}