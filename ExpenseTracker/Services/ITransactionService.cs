using ExpenseTracker.DTO;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface ITransactionService
{
    List<TransactionResponseDto> GetAllTransactions();
    ServiceResult<TransactionResponseDto> CreateTransaction(CreateTransactionRequestDto request);
    ServiceResult<TransactionResponseDto> UpdateTransaction(int transactionId, UpdateTransactionRequestDto request);
    ServiceResult<List<TransactionResponseDto>> GetTransactionsByCategory(int categoryId);
    ServiceResult<bool> DeleteTransaction(int id);
}