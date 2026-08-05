using System.Security.Claims;
using ExpenseTracker.DTO;
using ExpenseTracker.Extensions;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [Authorize]
    [HttpGet("allCategories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult GetAllCategories()
    {
        var userId = User.GetUserId();
        var categories = _categoryService.GetAllCategories(userId);
        return Ok(categories);
    }

    [Authorize]
    [HttpPost("createCategory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult CreateCategory(CreateCategoryRequestDto request)
    {
        var userId = User.GetUserId();
        var category = _categoryService.CreateCategory(userId, request);
        return Ok(category);
    }

    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult DeleteCategory(int id)
    {
        var userId = User.GetUserId();
        _categoryService.DeleteCategory(userId, id);
        return Ok();
    }

    [Authorize]
    [HttpPut("updateCategory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult UpdateCategory(int categoryId, UpdateCategoryRequestDto request)
    {
        var userId = User.GetUserId();
        _categoryService.UpdateCategory(userId, categoryId, request);
        return Ok();
    }
}