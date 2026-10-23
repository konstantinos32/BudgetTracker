using BudgetTracker.Models;
using BudgetTracker.Events;

namespace BudgetTracker.Services;

public class LoggerService
{
    private readonly string _logFilePath;
    private readonly TransactionService _transactionService;

    public LoggerService(
        TransactionService transactionService,
        string logsDirectory)
    {
        _transactionService = transactionService;

        Directory.CreateDirectory(logsDirectory);

        _logFilePath = Path.Combine(
            logsDirectory,
            "transactions.log");

        _transactionService.TransactionAdded += OnTransactionAdded;
    }

    private void OnTransactionAdded(
        object? sender,
        TransactionAddedEventArgs e)
    {
        Transaction transaction = e.Transaction;

        string logEntry =
            $"{transaction.Timestamp:O} | " +
            $"{transaction.Type} | " +
            $"{transaction.Description} | " +
            $"{transaction.Amount}";

        File.AppendAllText(
            _logFilePath,
            logEntry + Environment.NewLine);
    }
}