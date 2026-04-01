using System.ComponentModel.DataAnnotations;

namespace Spring2026_Project3_smthomas12.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; } = "";

        [Required]
        [Display(Name = "IMDB Link")]
        [Url]
        public string ImdbLink { get; set; } = "";

        [Required]
        [Display(Name = "Genre")]
        public string Genre { get; set; } = "";

        [Required]
        [Display(Name = "Year of Release")]
        [Range(1888, 2100)]
        public int Year { get; set; }

        [Display(Name = "Poster")]
        public byte[] Poster { get; set; } = Array.Empty<byte>();

        public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
    }
}