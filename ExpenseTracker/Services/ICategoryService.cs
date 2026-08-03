using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface ICategoryService
{
    List<CategoryResponseDto> GetAllCategories();
    ServiceResult<CategoryResponseDto> CreateCategory(CreateCategoryRequestDto request);
    ServiceResult <CategoryResponseDto> UpdateCategory(int categoryId, UpdateCategoryRequestDto request);
    ServiceResult<bool> DeleteCategory(int id);
}