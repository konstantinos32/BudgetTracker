using System.Collections.Generic;
using System.Globalization;
using BudgetTracker.Models;
using BudgetTracker.Services;

var storageService = new StorageService("data");
var transactionService = new TransactionService(storageService);
var loggerService = new LoggerService(transactionService, "logs");

bool isRunning = true;

while (isRunning)
{
    Console.WriteLine();
    Console.WriteLine("================================");
    Console.WriteLine("         BUDGET TRACKER");
    Console.WriteLine("================================");
    Console.WriteLine("1. Add income");
    Console.WriteLine("2. Add expense");
    Console.WriteLine("3. Remove transaction");
    Console.WriteLine("4. View report");
    Console.WriteLine("5. View transactions by type");
    Console.WriteLine("0. Exit");
    Console.WriteLine();

    Console.Write("Choose an option: ");
    string? choice = Console.ReadLine();

    switch (choice)
    {

case "1":
{
    string description = ReadDescription();
    decimal amount = ReadPositiveAmount();

    try
    {
        transactionService.AddTransaction(
            TransactionType.Income,
            description,
            amount);

        Console.WriteLine("Income added successfully.");
    }
    catch (IOException ex)
    {
        Console.WriteLine(
            $"Could not save income transaction: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine(
            $"Permission denied while saving transaction: {ex.Message}");
    }

    break;
}



case "2":
{
    string description = ReadDescription();
    decimal amount = ReadPositiveAmount();

    try
    {
        transactionService.AddTransaction(
            TransactionType.Expense,
            description,
            amount);

        Console.WriteLine("Expense added successfully.");
    }
    catch (IOException ex)
    {
        Console.WriteLine(
            $"Could not save expense transaction: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine(
            $"Permission denied while saving expense transaction: {ex.Message}");
    }

    break;
}


        case "3":
{
    DateOnly today = DateOnly.FromDateTime(DateTime.Now);

    TransactionReport report = transactionService.GetReport(
        today,
        today);

    DisplayTransactions(report.Transactions);

    if (report.Transactions.Count == 0)
    {
        break;
    }

    Console.Write("Enter the transaction ID to remove: ");
    string? input = Console.ReadLine();

    if (!Guid.TryParse(input, out Guid transactionId))
    {
        Console.WriteLine("Invalid transaction ID.");
        break;
    }



try
{
    bool removed = transactionService.RemoveTransaction(transactionId);

    if (removed)
    {
        Console.WriteLine("Transaction removed successfully.");
    }
    else
    {
        Console.WriteLine("Transaction not found.");
    }
}
catch (InvalidDataException ex)
{
    Console.WriteLine($"Could not read transaction data: {ex.Message}");
}
catch (IOException ex)
{
    Console.WriteLine($"Could not access transaction files: {ex.Message}");
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Access denied: {ex.Message}");
}


    break;
}


case "4":
{
    DateOnly startDate = ReadDate("Start date (TT.MM.JJJJ): ");
    DateOnly endDate = ReadDate("End date (TT.MM.JJJJ): ");

    try
    {
        TransactionReport report = transactionService.GetReport(
            startDate,
            endDate);

        Console.WriteLine();
        Console.WriteLine("========== REPORT ==========");

        DisplayTransactions(report.Transactions);

        Console.WriteLine("----------------------------");
        Console.WriteLine($"Total income:   {report.TotalIncome:C}");
        Console.WriteLine($"Total expenses: {report.TotalExpenses:C}");
        Console.WriteLine($"Balance:        {report.Balance:C}");
    }


catch (ArgumentException ex)
{
    Console.WriteLine($"Could not generate report: {ex.Message}");
}
catch (InvalidDataException ex)
{
    Console.WriteLine($"Could not read transaction data: {ex.Message}");
}
catch (IOException ex)
{
    Console.WriteLine($"Could not read transaction data: {ex.Message}");
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Access denied while reading transaction data: {ex.Message}");
}



    break;
}




case "5":
{
    DateOnly startDate = ReadDate("Start date (TT.MM.JJJJ): ");
DateOnly endDate = ReadDate("End date (TT.MM.JJJJ): ");

if (startDate > endDate)
{
    Console.WriteLine(
        "The start date cannot be later than the end date.");
    break;
}

    Console.WriteLine("1. Income");
    Console.WriteLine("2. Expense");
    Console.Write("Choose transaction type: ");

    string? typeChoice = Console.ReadLine();

    TransactionType type;

    if (typeChoice == "1")
    {
        type = TransactionType.Income;
    }
    else if (typeChoice == "2")
    {
        type = TransactionType.Expense;
    }
    else
    {
        Console.WriteLine("Invalid transaction type.");
        break;
    }

    try
    {
        List<Transaction> transactions =
            transactionService.GetTransactionsByType(
                startDate,
                endDate,
                type);

        Console.WriteLine();
        Console.WriteLine(
            $"========== {type.ToString().ToUpper()} TRANSACTIONS ==========");

        DisplayTransactions(transactions);
    }


catch (ArgumentException ex)
{
    Console.WriteLine($"Could not generate report: {ex.Message}");
}
catch (InvalidDataException ex)
{
    Console.WriteLine($"Could not read transaction data: {ex.Message}");
}
catch (IOException ex)
{
    Console.WriteLine($"Could not read transaction data: {ex.Message}");
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Access denied while reading transaction data: {ex.Message}");
}



    break;
}



        case "0":
            isRunning = false;
            Console.WriteLine("Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid option. Please choose 0–5.");
            break;
    }
}

static string ReadDescription()
{
    while (true)
    {
        Console.Write("Description: ");
        string? description = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(description))
        {
            return description.Trim();
        }

        Console.WriteLine("Description cannot be empty.");
    }
}

static decimal ReadPositiveAmount()
{
    while (true)
    {
        Console.Write("Amount: ");
        string? input = Console.ReadLine();

        if (decimal.TryParse(
            input,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out decimal amount) && amount > 0)
        {
            return amount;
        }

        Console.WriteLine("Please enter a valid amount greater than zero.");
    }
}

static void DisplayTransactions(List<Transaction> transactions)
{
    if (transactions.Count == 0)
    {
        Console.WriteLine("No transactions found.");
        return;
    }

    foreach (Transaction transaction in transactions)
    {
        Console.WriteLine(
            $"ID: {transaction.Id} | " +
            $"{transaction.Timestamp:dd.MM.yyyy HH:mm} | " +
            $"{transaction.Type} | " +
            $"{transaction.Description} | " +
            $"{transaction.Amount:C}");
    }
}


static DateOnly ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (DateOnly.TryParseExact(
            input,
            "dd.MM.yyyy",
            CultureInfo.GetCultureInfo("de-DE"),
            DateTimeStyles.None,
            out DateOnly date))
        {
            return date;
        }

        Console.WriteLine(
            "Invalid date. Please use the format TT.MM.JJJJ, for example 01.10.2026.");
    }
}
