namespace FinTrack.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public TransactionType Type { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
