using ExpenseTracker.Data;
using ExpenseTracker.DTO;
using ExpenseTracker.Exceptions;
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
    
    public List<TransactionResponseDto> GetAllTransactions(int userId)
    {
        return _context.Transactions
            .Where(x => x.UserId == userId)
            .Select(x=> new TransactionResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Amount = x.Amount,
                Description = x.Description,
                Type = x.Type,
                Date = x.Date,
                Category = x.Category != null ? x.Category.Name : null,
            }).ToList();
    }

    public void UpdateBalance(User user, TransactionType type, decimal amount)
    {
        switch (type)
        {
            case TransactionType.Income:
                user.Balance += amount;
                break;
            
            case TransactionType.Outcome:
                if (user.Balance < amount)
                    throw new BusinessException("Недостаточно средств");
                
                user.Balance -= amount;
                break;
        }
    }

    public void ReverseBalance(User user, TransactionType type, decimal amount)
    {
        switch (type)
        {
            case TransactionType.Income:
                user.Balance -= amount;
                break;
            
            case TransactionType.Outcome:
                user.Balance += amount;
                break;
        }
    }

    public void UpdateTransactionBalance(User user, TransactionType oldType, decimal oldAmount, Transaction updatedTransaction)
    {
        var balanceAfterReverse =  user.Balance;
        
        if (oldType == TransactionType.Income)
            balanceAfterReverse -= oldAmount;
        else 
            balanceAfterReverse += oldAmount;

        if (updatedTransaction.Type == TransactionType.Outcome && balanceAfterReverse < updatedTransaction.Amount)
            throw new BusinessException("Недостаточно средств");
        
        ReverseBalance(user, oldType, oldAmount);
        UpdateBalance(user, updatedTransaction.Type, updatedTransaction.Amount);
    }

    public TransactionResponseDto CreateTransaction(int userId, CreateTransactionRequestDto request)
    {
        var user = _context.Users.SingleOrDefault(u=>u.Id == userId);

        if (user == null)
            throw new BusinessException("Пользователь не найден");
        
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BusinessException("Название транзакции не может быть пустым.");

        if (request.Amount <= 0)
            throw new BusinessException("Сумма должна быть больше нуля.");

        if (request.Date > DateTime.UtcNow)
            throw new BusinessException("Нельзя создать транзакцию на еще не наступившую дату.");
        
        var category = _context.Categories.FirstOrDefault(c => c.Id == request.CategoryId && (c.UserId==userId || c.UserId==null));
        if (request.CategoryId.HasValue && category == null)
            throw new BusinessException("Указанной категории не существует.");

        var transaction = new Transaction(
            userId,
            request.Name,
            request.Description,
            request.Amount,
            request.Date,
            request.Type,
            category?.Id);
        
        UpdateBalance(user, request.Type, request.Amount);
        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return new TransactionResponseDto()
        {
            Name = request.Name,
            Amount = request.Amount,
            Date = request.Date,
            Type = request.Type,
        };
    }

    public TransactionResponseDto UpdateTransaction(int userId, int transactionId, UpdateTransactionRequestDto request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Id == userId);
        if (user == null)
            throw new BusinessException("Пользователь не найден");
        
        var transaction = _context.Transactions.Include(transaction => transaction.Category).FirstOrDefault(t => t.Id == transactionId && t.UserId == userId);
        if (transaction == null)
            throw new BusinessException($"Транзакции с айди {transactionId} не существует.");
        
        var oldAmount = transaction.Amount;
        var oldType = transaction.Type;
        
        if (!string.IsNullOrWhiteSpace(request.Name))
            transaction.Name = request.Name.Trim();

        if (request.Amount.HasValue)
        {
            if (request.Amount <= 0)
                throw new BusinessException("Сумма должна быть больше нуля.");
            
            transaction.Amount = request.Amount.Value;
        }

        if (request.Date.HasValue)
        {
            if (request.Date.Value > DateTime.UtcNow)
                throw new BusinessException("Нельзя создать транзакцию на еще не наступившую дату.");
            
            transaction.Date = request.Date.Value;
        }
        
        if (request.Description != null)
            transaction.Description = request.Description;

        if (request.RemoveCategory)
        {
            transaction.CategoryId = null;
            transaction.Category = null;
        }
        else if (request.CategoryId.HasValue)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == request.CategoryId.Value &&  (c.UserId==userId || c.UserId==null));
            if (category == null)
                throw new BusinessException("Указанной категории не существует");
            transaction.Category = category;
            transaction.CategoryId = category.Id;
        }
        
        if (request.Type.HasValue)
            transaction.Type = request.Type.Value;

        if (oldAmount != transaction.Amount || oldType != transaction.Type)
            UpdateTransactionBalance(user, oldType, oldAmount, transaction);
        
        _context.SaveChanges();

        return new TransactionResponseDto()
        {
            Name = transaction.Name,
            Amount = transaction.Amount,
            Category = transaction.Category?.Name,
            Date = transaction.Date,
            Description = transaction.Description,
            Type = transaction.Type,
        };
    }

    public List<TransactionResponseDto> GetTransactionsByCategory(int userId, int categoryId)
    {
        var categoryExists =  _context.Categories.Any(c => c.Id == categoryId && (c.UserId==userId || c.UserId==null));
        if (!categoryExists)
            throw new BusinessException("Указанной категории не существует для вывода транзакций");
        
        var transactions = _context.Transactions.Where(t => t.CategoryId == categoryId && t.UserId==userId).Include(t => t.Category).ToList();

        return transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Amount = t.Amount,
            Date = t.Date,
            Type = t.Type,
            Category = t.Category?.Name
        }).ToList();
    }

    public void DeleteTransaction(int userId, int id)
    {
        var user =  _context.Users.SingleOrDefault(u => u.Id == userId);
        if (user == null)
            throw new BusinessException("Пользователь не найден");
        
        var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id && t.UserId==userId);
        if (transaction == null)
            throw new BusinessException("Указанной транзакции не существует.");
        
        ReverseBalance(user, transaction.Type, transaction.Amount);
        
        _context.Transactions.Remove(transaction);
        _context.SaveChanges();
    }
}