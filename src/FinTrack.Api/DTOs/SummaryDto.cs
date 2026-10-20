namespace FinTrack.Api.DTOs
{
    public class SummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }
        public List<CategoryTotalDto> ExpensesByCategory { get; set; } = new List<CategoryTotalDto>();
    }

    public class CategoryTotalDto
    {
        public string Category { get; set; } = "";
        public decimal Total { get; set; }
    }
}
