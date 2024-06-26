

using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApi.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; } // becuase the max does not > 250
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
