using System.ComponentModel.DataAnnotations;
using FinTrack.Api.Models;

namespace FinTrack.Api.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public TransactionType Type { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "";

        [Required]
        public TransactionType Type { get; set; }
    }
}
