using System.Text.Json;
using BudgetTracker.Events;
using BudgetTracker.Models;

namespace BudgetTracker.Services;

public class TransactionService
{
    private readonly StorageService _storageService;
    public event EventHandler<TransactionAddedEventArgs>? TransactionAdded;
    public TransactionService(StorageService storageService)
    {
        _storageService = storageService;
    }

    public Transaction AddTransaction(
        TransactionType type,
        string description,
        decimal amount
    )
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be greater than zero"
            );
        }

var transaction = new Transaction
{
    Id = Guid.NewGuid(),
    Timestamp = DateTimeOffset.Now,
    Type = type,
    Description = description,
    Amount = amount
};

DateTime transactionDate = transaction.Timestamp.Date;

List<Transaction> transactions =
_storageService.LoadTransactions(transactionDate);
transactions.Add(transaction);

_storageService.SaveTransactions(transactionDate, transactions);

TransactionAdded?.Invoke(
    this,
    new TransactionAddedEventArgs(transaction)
);

return transaction;   

 }
 public bool RemoveTransaction(Guid id)
{
    foreach (string filePath in _storageService.GetAllTransactionFiles())
    {
        string json = File.ReadAllText(filePath);

        List<Transaction>? transactions =
            JsonSerializer.Deserialize<List<Transaction>>(json);

        if (transactions is null)
        {
            continue;
        }

        Transaction? transaction = transactions.FirstOrDefault(
            t => t.Id == id);

        if (transaction is null)
        {
            continue;
        }

        transactions.Remove(transaction);

        DateTime transactionDate = transaction.Timestamp.Date;

        _storageService.SaveTransactions(
            transactionDate,
            transactions);

        return true;
    }

    return false;
}
}

