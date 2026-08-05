using ExpenseTracker.DTO;
using ExpenseTracker.Extensions;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [Authorize]
    [HttpGet("all-transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult GetAllTransactions()
    {
        var userId = User.GetUserId();
        return Ok(_transactionService.GetAllTransactions(userId));
    }

    [Authorize]
    [HttpPost("new-transaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult CreateTransaction(CreateTransactionRequestDto request)
    {
        var userId = User.GetUserId();
        return Ok(_transactionService.CreateTransaction(userId, request));
    }

    [Authorize]
    [HttpPut("edit-transaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult UpdateTransaction(int transactionId, UpdateTransactionRequestDto request)
    {
        var userId = User.GetUserId();
        var response = _transactionService.UpdateTransaction(userId, transactionId, request);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult GetTransactionsByCategory(int categoryId)
    {
        var userId = User.GetUserId();
        return Ok(_transactionService.GetTransactionsByCategory(userId, categoryId));
    }

    [Authorize]
    [HttpDelete("delete-transaction/{transactionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult DeleteTransaction(int transactionId)
    {
        var userId = User.GetUserId();
        _transactionService.DeleteTransaction(userId, transactionId);
        return Ok();
    }
}