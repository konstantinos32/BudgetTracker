using System.Linq;
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

public TransactionReport GetReport(DateOnly startDate, DateOnly endDate)
{
    if (startDate > endDate)
    {
        throw new ArgumentException(
            "Start date cannot be later than end date.");
    }

    List<Transaction> allTransactions = _storageService
        .GetAllTransactionFiles()
        .SelectMany(filePath =>
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<Transaction>>(json)
                ?? new List<Transaction>();
        })
        .ToList();

    List<Transaction> filteredTransactions = allTransactions
        .Where(transaction =>
            DateOnly.FromDateTime(transaction.Timestamp.Date) >= startDate &&
            DateOnly.FromDateTime(transaction.Timestamp.Date) <= endDate)
        .ToList();

    decimal totalIncome = filteredTransactions
        .Where(transaction => transaction.Type == TransactionType.Income)
        .Sum(transaction => transaction.Amount);

    decimal totalExpenses = filteredTransactions
        .Where(transaction => transaction.Type == TransactionType.Expense)
        .Sum(transaction => transaction.Amount);

    return new TransactionReport
    {
        Transactions = filteredTransactions,
        TotalIncome = totalIncome,
        TotalExpenses = totalExpenses
    };
}

public List<Transaction> GetTransactionsByType(
    DateOnly startDate,
    DateOnly endDate,
    TransactionType type)
{
    return GetReport(startDate, endDate)
        .Transactions
        .Where(transaction => transaction.Type == type)
        .ToList();
}
}

