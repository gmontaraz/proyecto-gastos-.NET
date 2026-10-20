using FinTrack.Api.DTOs;

namespace FinTrack.Api.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAll(int userId);
        Task<CategoryDto> Create(int userId, CreateCategoryDto dto);
        Task<bool> Delete(int userId, int id);
    }
}
