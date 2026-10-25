using BudgetTracker.Events;
using BudgetTracker.Models;

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
            "transactions.log"
        );

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

        try
        {
            File.AppendAllText(
                _logFilePath,
                logEntry + Environment.NewLine
            );
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine(
                $"Warning: The transaction was saved, but logging failed: {ex.Message}"
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine(
                $"Warning: The transaction was saved, but the log file could not be accessed: {ex.Message}"
            );
        }
    }
}