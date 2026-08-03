using ExpenseTracker.Data;
using ExpenseTracker.DTO;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public List<TransactionResponseDto> GetAllTransactions()
    {
        var transactions = _context.Transactions.Include(t=>t.Category).ToList();
        var transactionsDto = transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Amount = t.Amount,
            Date = t.Date,
            Type = t.Type,
            Category = t.Category?.Name,
        }).ToList();
        
        return transactionsDto;
    }

    public ServiceResult<TransactionResponseDto> CreateTransaction(CreateTransactionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new ServiceResult<TransactionResponseDto>()
            {
                Success = false,
                Message = "Название транзакции не может быть пустым."
            };
        }

        if (request.Amount <= 0)
        {
            return new ServiceResult<TransactionResponseDto>()
            {
                Success = false,
                Message = "Сумма должна быть больше нуля."
            };
        }

        if (request.Date > DateTime.UtcNow)
        {
            return new ServiceResult<TransactionResponseDto>()
            {
                Success = false,
                Message = "Нельзя создать транзакцию на еще не наступившую дату."
            };
        }
        
        var category = _context.Categories.FirstOrDefault(c => c.Id == request.CategoryId);
        if (category == null)
        {
            return new ServiceResult<TransactionResponseDto>()
            {
                Success = false,
                Message = "Указанной категории не существует."
            };
        }

        if (request.Type == TransactionType.Income)
        {
            
        }
        else
        {
            
        }

        var transaction = new Transaction(
            null,
            request.Name,
            request.Description,
            request.Amount,
            request.Date,
            request.Type,
            category.Id);
        
        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return new ServiceResult<TransactionResponseDto>()
        {
            Success = true,
            Data = new TransactionResponseDto
            {
                Amount = request.Amount,
                Date = request.Date,
                Type = request.Type,
            }
        };
    }

    public ServiceResult<TransactionResponseDto> UpdateTransaction(int transactionId, UpdateTransactionRequestDto request)
    {
        var transaction = _context.Transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction == null)
        {
            return new ServiceResult<TransactionResponseDto>()
            {
                Success = false,
                Message = $"Транзакции с айди {transactionId} не существует."
            };
        }
        
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            transaction.Name = request.Name.Trim();
        }
        
        transaction.Name = request.Name;

        if (request.Amount.HasValue)
        {
            if (request.Amount <= 0)
            {
                return new ServiceResult<TransactionResponseDto>()
                {
                    Success = false,
                    Message = "Сумма должна быть больше нуля."
                };
            }
        }
        
        transaction.Amount = request.Amount.Value;

        if (request.Date.HasValue)
        {
            if (request.Date > DateTime.UtcNow)
            {
                return new ServiceResult<TransactionResponseDto>()
                {
                    Success = false,
                    Message = "Нельзя создать транзакцию на еще не наступившую дату."
                };
            }
        }
        
        transaction.Date = request.Date.Value;

        if (request.CategoryId.HasValue)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == request.CategoryId);
            if (category == null)
            {
                return new ServiceResult<TransactionResponseDto>()
                {
                    Success = false,
                    Message = "Указанной категории не существует."
                };
            }
            transaction.Category = category;
        }

        if (request.Type == TransactionType.Income)
        {
            
        }
        else
        {
            
        }
        
        _context.Transactions.Update(transaction);
        _context.SaveChanges();

        return new ServiceResult<TransactionResponseDto>()
        {
            Success = true,
            Data = new TransactionResponseDto
            {

            }
        };
    }

    public ServiceResult<List<TransactionResponseDto>> GetTransactionsByCategory(int categoryId)
    {
        var categoryExists =  _context.Categories.Any(c => c.Id == categoryId);
        if (!categoryExists)
        {
            return new ServiceResult<List<TransactionResponseDto>>()
            {
                Success = false,    
                Message = "Категории с айди {categoryId} не существует."
            };
        }
        
        var transactions = _context.Transactions.Where(t => t.CategoryId == categoryId).Include(t => t.Category).ToList();

        var transactionsDto = transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Amount = t.Amount,
            Date = t.Date,
            Type = t.Type,
            Category = t.Category?.Name
        }).ToList();

        return new ServiceResult<List<TransactionResponseDto>>
        {
            Success = true,
            Data = transactionsDto
        };
    }

    public ServiceResult<bool> DeleteTransaction(int id)
    {
        var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null)
        {
            return new ServiceResult<bool>()
            {
                Success = false,
                Message = "Указанной транзакции не существует."
            };
        }
        
        _context.Transactions.Remove(transaction);
        _context.SaveChanges();

        return new ServiceResult<bool>()
        {
            Success = true,
            Message = "Транзакция успешно удалена."
        };
    }
}