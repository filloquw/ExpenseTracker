using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface ICategoryService
{
    List<CategoryResponseDto> GetAllCategories(int userId);
    CategoryResponseDto CreateCategory(int userId, CreateCategoryRequestDto request);
    CategoryResponseDto UpdateCategory(int userId, int categoryId, UpdateCategoryRequestDto request);
    void DeleteCategory(int userId, int id);
}