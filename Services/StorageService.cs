using System.Linq;
using BudgetTracker.Models;
using System.Text.Json;

namespace BudgetTracker.Services;
public class StorageService
{
    private readonly string  _dataDirectory;
    public  StorageService (string dataDirectory)
    {
        _dataDirectory=dataDirectory;

        Directory.CreateDirectory(_dataDirectory);

        
    }

public string GetFilePath(DateTime transactionDate)
    {
        return Path.Combine (
            _dataDirectory,
            transactionDate.ToString("yyyy-MM-dd") + ".json"
        );
    }

    public void SaveTransactions(
        DateTime transactionDate,
        List<Transaction>transactions)
    {
        string filepath= GetFilePath(transactionDate);
        string json = JsonSerializer.Serialize(transactions);
        File.WriteAllText(filepath, json);
    }

   public List<Transaction> LoadTransactions(DateTime transactionDate)
{
    string filePath = GetFilePath(transactionDate);

    if (!File.Exists(filePath))
    {
        return new List<Transaction>();
    }

    string json = File.ReadAllText(filePath);
   List<Transaction>? transactions =
    JsonSerializer.Deserialize<List<Transaction>>(json);
    return transactions ?? new List<Transaction>();
}
public List<string> GetAllTransactionFiles()
{
    return Directory.GetFiles(_dataDirectory, "*.json").ToList();
}
}