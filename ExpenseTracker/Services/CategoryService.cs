using ExpenseTracker.Data;
using ExpenseTracker.DTO;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public List<CategoryResponseDto> GetAllCategories(int  userId)
    {
        return _context.Categories
            .Where(c => c.UserId == userId || c.UserId == null)
            .Select(c=> new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            })
            .ToList();
    }

    public CategoryResponseDto CreateCategory(int userId, CreateCategoryRequestDto request)
    {
        var normalizedName = request.Name.Trim().ToLower();
        var exists = _context.Categories.Any(c=> c.Name.ToLower() == normalizedName&&(c.UserId == userId || c.UserId == null));

        if (exists)
            throw new BusinessException("Такая категория уже существует");

        var name = request.Name.Trim();
        
        var category = new Category(
            userId,
            name,
            request.Description);
        
        _context.Categories.Add(category);
        _context.SaveChanges();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
    }

    public CategoryResponseDto UpdateCategory(int userId, int categoryId, UpdateCategoryRequestDto request)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId && (c.UserId == userId || c.UserId == null));
        if (category == null)
            throw new BusinessException("Такой категории не существует");
        
        if (category.UserId != userId)
            throw new BusinessException("Категория является системной, ее нельзя поменять");

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var newName = request.Name.Trim();
            if (newName != category.Name)
            {
                var exists = _context.Categories.Any(c=> c.Name.ToLower() == newName.ToLower() && (c.UserId == userId || c.UserId == null));

                if (exists)
                {
                    throw new BusinessException("Категория с таким именем уже существует");
                }
                
                category.Name = newName;
            }
        }

        if (request.Description != null)
            category.Description = request.Description;
        
        _context.SaveChanges();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
    }

    public void DeleteCategory(int userId, int id)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == id);

        if (category == null)
            throw new BusinessException("Категории с таким айди нет");

        if (category.UserId == null)
            throw new BusinessException("Вы не можете удалить системную категорию");
        
        if (category.UserId != userId)
            throw new BusinessException("Вы не можете удалить чужую категорию");
        
        _context.Categories.Remove(category);
        _context.SaveChanges();
    }
}