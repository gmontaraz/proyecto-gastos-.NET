using System.ComponentModel.DataAnnotations;
using FinTrack.Api.Models;

namespace FinTrack.Api.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

    public class CreateTransactionDto
    {
        [Required]
        [MaxLength(100)]
        public string Description { get; set; } = "";

        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
