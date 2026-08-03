using ExpenseTracker.Data;
using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public List<CategoryResponseDto> GetAllCategories()
    {
        var categories = _context.Categories.ToList();
        var categoriesDto = categories.Select(c=> new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
        }).ToList();
        
        return categoriesDto;
    }

    public ServiceResult<CategoryResponseDto> CreateCategory(CreateCategoryRequestDto request)
    {
        var normalizedName = request.Name.Trim().ToLower();
        var exists = _context.Categories.Any(c=> c.Name.ToLower() == normalizedName);

        if (exists)
        {
            return new ServiceResult<CategoryResponseDto>
            {
                Success = false,
                Message = $"Категория с названием {request.Name} уже существует."
            };
        }

        var name = request.Name.Trim();
        
        var category = new Category(
            null,
            name,
            request.Description);
        
        _context.Categories.Add(category);
        _context.SaveChanges();

        return new ServiceResult<CategoryResponseDto>
        {
            Success = true,
            Data = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
            }
        };
    }

    public ServiceResult<CategoryResponseDto> UpdateCategory(int categoryId, UpdateCategoryRequestDto request)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null)
        {
            return new ServiceResult<CategoryResponseDto>
            {
                Success = false,
                Message = $"Категории с айди: {categoryId} не существует."
            };
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var newName = request.Name.Trim();
            if (newName != category.Name)
            {
                var exists = _context.Categories.Any(c=> c.Name.ToLower() == newName.ToLower());

                if (exists)
                {
                    return new ServiceResult<CategoryResponseDto>
                    {
                        Success = false,
                        Message = $"Категория с названием {newName} уже существует."
                    };
                }
                
                category.Name = newName;
            }
        }

        if (request.Description != null)
        {
            category.Description = request.Description;
        }
        
        _context.SaveChanges();

        return new ServiceResult<CategoryResponseDto>
        {
            Success = true,
            Data = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
            }
        };
    }

    public ServiceResult<bool> DeleteCategory(int id)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = $"Категории с айди {id} не существует."
            };
        }
        
        _context.Categories.Remove(category);
        _context.SaveChanges();

        return new ServiceResult<bool>
        {
            Success = true,
            Message = "Категория успешно удалена."
        };
    }
}