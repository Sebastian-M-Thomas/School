using System.ComponentModel.DataAnnotations;

namespace Spring2026_Project3_smthomas12.Models
{
    public class ActorMovie
    {
        public int Id { get; set; }

        [Required]
        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}