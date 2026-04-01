using System.ComponentModel.DataAnnotations;

namespace Spring2026_Project3_smthomas12.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = "";

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = "";

        [Required]
        [Display(Name = "Age")]
        [Range(0, 150)]
        public int Age { get; set; }

        [Required]
        [Display(Name = "IMDB Link")]
        [Url]
        public string ImdbLink { get; set; } = "";

        [Display(Name = "Photo")]
        public byte[] Photo { get; set; } = Array.Empty<byte>();

        public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
    }
}