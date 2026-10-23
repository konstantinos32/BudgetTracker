using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BudgetTracker.Models;

namespace BudgetTracker.Services;

public class StorageService
{
    private readonly string _dataDirectory;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true
    };

    public StorageService(string dataDirectory)
    {
        _dataDirectory = dataDirectory;

        Directory.CreateDirectory(_dataDirectory);
    }

    public string GetFilePath(DateTime transactionDate)
    {
        return Path.Combine(
            _dataDirectory,
            transactionDate.ToString("yyyy-MM-dd") + ".json"
        );
    }

    public void SaveTransactions(
        DateTime transactionDate,
        List<Transaction> transactions)
    {
        string filePath = GetFilePath(transactionDate);

        string json = JsonSerializer.Serialize(
            transactions,
            _jsonOptions
        );

        File.WriteAllText(filePath, json);
    }

    public List<Transaction> LoadTransactions(DateTime transactionDate)
    {
        string filePath = GetFilePath(transactionDate);

        if (!File.Exists(filePath))
        {
            return new List<Transaction>();
        }

        return LoadTransactionsFromFile(filePath);
    }


public List<Transaction> LoadTransactionsFromFile(string filePath)
{
    string json = File.ReadAllText(filePath);

    try
    {
        List<Transaction>? transactions =
            JsonSerializer.Deserialize<List<Transaction>>(
                json,
                _jsonOptions);

        return transactions ?? new List<Transaction>();
    }
    catch (JsonException ex)
    {
        throw new InvalidDataException(
            $"The transaction file '{filePath}' contains invalid JSON.",
            ex);
    }
}


    public List<string> GetAllTransactionFiles()
    {
        return Directory.GetFiles(_dataDirectory, "*.json").ToList();
    }
}