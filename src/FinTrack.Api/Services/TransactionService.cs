using FinTrack.Api.Data;
using FinTrack.Api.DTOs;
using FinTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> GetByMonth(int userId, int year, int month)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date.Year == year && t.Date.Month == month)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return transactions.Select(t => ToDto(t)).ToList();
        }

        public async Task<TransactionDto?> GetById(int userId, int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
                return null;

            return ToDto(transaction);
        }

        public async Task<TransactionDto?> Create(int userId, CreateTransactionDto dto)
        {
            // comprobar que la categoria es del usuario
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
            if (category == null)
                return null;

            var transaction = new Transaction
            {
                Description = dto.Description,
                Amount = dto.Amount,
                Date = dto.Date,
                Type = category.Type, // el tipo depende de la categoria
                CategoryId = category.Id,
                UserId = userId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            transaction.Category = category;
            return ToDto(transaction);
        }

        public async Task<TransactionDto?> Update(int userId, int id, CreateTransactionDto dto)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return null;

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
            if (category == null)
                return null;

            transaction.Description = dto.Description;
            transaction.Amount = dto.Amount;
            transaction.Date = dto.Date;
            transaction.CategoryId = category.Id;
            transaction.Type = category.Type;

            await _context.SaveChangesAsync();

            transaction.Category = category;
            return ToDto(transaction);
        }

        public async Task<bool> Delete(int userId, int id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SummaryDto> GetSummary(int userId, int year, int month)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date.Year == year && t.Date.Month == month)
                .ToListAsync();

            // lo calculo en memoria porque SQLite no deja hacer Sum() con decimal
            var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            var summary = new SummaryDto
            {
                TotalIncome = income,
                TotalExpenses = expenses,
                Balance = income - expenses
            };

            var groups = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category!.Name);

            foreach (var group in groups)
            {
                summary.ExpensesByCategory.Add(new CategoryTotalDto
                {
                    Category = group.Key,
                    Total = group.Sum(t => t.Amount)
                });
            }

            return summary;
        }

        private static TransactionDto ToDto(Transaction t)
        {
            return new TransactionDto
            {
                Id = t.Id,
                Description = t.Description,
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type,
                CategoryId = t.CategoryId,
                CategoryName = t.Category != null ? t.Category.Name : ""
            };
        }
    }
}
