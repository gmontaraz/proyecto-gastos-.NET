using FinTrack.Api.DTOs;

namespace FinTrack.Api.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionDto>> GetByMonth(int userId, int year, int month);
        Task<TransactionDto?> GetById(int userId, int id);
        Task<TransactionDto?> Create(int userId, CreateTransactionDto dto);
        Task<TransactionDto?> Update(int userId, int id, CreateTransactionDto dto);
        Task<bool> Delete(int userId, int id);
        Task<SummaryDto> GetSummary(int userId, int year, int month);
    }
}
