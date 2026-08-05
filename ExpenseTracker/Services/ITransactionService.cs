using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface ITransactionService
{
    List<TransactionResponseDto> GetAllTransactions(int userId);
    TransactionResponseDto CreateTransaction(int userId, CreateTransactionRequestDto request);
    TransactionResponseDto UpdateTransaction(int userId, int transactionId, UpdateTransactionRequestDto request);
    void UpdateBalance(User user, TransactionType type, decimal amount);
    void ReverseBalance(User user, TransactionType type, decimal amount);
    
    void UpdateTransactionBalance(User user, TransactionType oldType, decimal oldAmount, Transaction updatedTransaction);
    List<TransactionResponseDto> GetTransactionsByCategory(int userId, int categoryId);
    void DeleteTransaction(int userId, int id);
}