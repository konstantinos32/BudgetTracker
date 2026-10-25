using System.Globalization;
using BudgetTracker.Models;
using BudgetTracker.Services;

StorageService storageService;
TransactionService transactionService;
LoggerService loggerService;

try
{
    storageService = new StorageService("data");
    transactionService = new TransactionService(storageService);
    loggerService = new LoggerService(transactionService, "logs");
}
catch (UnauthorizedAccessException ex)
{
    Console.Error.WriteLine(
        $"Startup failed: Access denied while creating application directories. {ex.Message}"
    );
    return;
}
catch (IOException ex)
{
    Console.Error.WriteLine(
        $"Startup failed: Could not access the application directories. {ex.Message}"
    );
    return;
}

bool isRunning = true;

try
{
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

        if (choice is null)
        {
            Console.WriteLine();
            Console.WriteLine("Input ended. Goodbye!");
            break;
        }

        switch (choice)
        {
            case "1":
            {
                string description = ReadDescription();
                decimal amount = ReadPositiveAmount();

                try
                {
                    Transaction transaction = transactionService.AddTransaction(
                        TransactionType.Income,
                        description,
                        amount
                    );

                    Console.WriteLine(
                        $"Income added successfully: {transaction.Amount.ToString("C", CultureInfo.GetCultureInfo("de-DE"))}"
                    );
                }
                catch (InvalidDataException ex)
                {
                    Console.WriteLine($"Invalid transaction data: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Could not save the transaction: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid transaction: {ex.Message}");
                }

                break;
            }

            case "2":
            {
                string description = ReadDescription();
                decimal amount = ReadPositiveAmount();

                try
                {
                    Transaction transaction = transactionService.AddTransaction(
                        TransactionType.Expense,
                        description,
                        amount
                    );

                    Console.WriteLine(
                        $"Expense added successfully: {transaction.Amount.ToString("C", CultureInfo.GetCultureInfo("de-DE"))}"
                    );
                }
                catch (InvalidDataException ex)
                {
                    Console.WriteLine($"Invalid transaction data: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Could not save the transaction: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid transaction: {ex.Message}");
                }

                break;
            }

            case "3":
            {
                try
                {
                    List<Transaction> transactions =
                        transactionService.GetAllTransactions();

                    if (transactions.Count == 0)
                    {
                        Console.WriteLine("No transactions available to remove.");
                        break;
                    }

                    DisplayTransactions(transactions);

                    Console.Write("Enter the transaction ID to remove: ");
                    string? input = Console.ReadLine();

                    if (input is null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Input ended. Goodbye!");
                        isRunning = false;
                        break;
                    }

                    if (!Guid.TryParse(input, out Guid transactionId))
                    {
                        Console.WriteLine("Invalid transaction ID.");
                        break;
                    }

                    bool removed =
                        transactionService.RemoveTransaction(transactionId);

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
                    Console.WriteLine(
                        $"Could not read transaction data: {ex.Message}"
                    );
                }
                catch (IOException ex)
                {
                    Console.WriteLine(
                        $"Could not access transaction files: {ex.Message}"
                    );
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

                if (startDate > endDate)
                {
                    Console.WriteLine(
                        "The start date cannot be later than the end date."
                    );
                    break;
                }

                try
                {
                    TransactionReport report = transactionService.GetReport(
                        startDate,
                        endDate
                    );

                    Console.WriteLine();
                    Console.WriteLine(
                        $"Report from {startDate:dd.MM.yyyy} to {endDate:dd.MM.yyyy}"
                    );
                    Console.WriteLine("--------------------------------");

                    DisplayTransactions(report.Transactions);

                    Console.WriteLine("--------------------------------");
                    Console.WriteLine(
                        $"Total income:   {report.TotalIncome.ToString("C", CultureInfo.GetCultureInfo("de-DE"))}"
                    );
                    Console.WriteLine(
                        $"Total expenses: {report.TotalExpenses.ToString("C", CultureInfo.GetCultureInfo("de-DE"))}"
                    );
                    Console.WriteLine(
                        $"Balance:        {report.Balance.ToString("C", CultureInfo.GetCultureInfo("de-DE"))}"
                    );
                }
                catch (InvalidDataException ex)
                {
                    Console.WriteLine(
                        $"Could not read transaction data: {ex.Message}"
                    );
                }
                catch (IOException ex)
                {
                    Console.WriteLine(
                        $"Could not access transaction files: {ex.Message}"
                    );
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid report request: {ex.Message}");
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
                        "The start date cannot be later than the end date."
                    );
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("Choose transaction type:");
                Console.WriteLine("1. Income");
                Console.WriteLine("2. Expense");
                Console.Write("Your choice: ");

                string? typeChoice = Console.ReadLine();

                if (typeChoice is null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Input ended. Goodbye!");
                    isRunning = false;
                    break;
                }

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
                            type
                        );

                    Console.WriteLine();
                    Console.WriteLine($"Transactions of type: {type}");
                    Console.WriteLine("--------------------------------");

                    if (transactions.Count == 0)
                    {
                        Console.WriteLine(
                            "No transactions found for this period."
                        );
                    }
                    else
                    {
                        DisplayTransactions(transactions);
                    }
                }
                catch (InvalidDataException ex)
                {
                    Console.WriteLine(
                        $"Could not read transaction data: {ex.Message}"
                    );
                }
                catch (IOException ex)
                {
                    Console.WriteLine(
                        $"Could not access transaction files: {ex.Message}"
                    );
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid report request: {ex.Message}");
                }

                break;
            }

            case "0":
                isRunning = false;
                Console.WriteLine("Goodbye!");
                break;

            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
}
catch (EndOfStreamException)
{
    Console.WriteLine();
    Console.WriteLine("Input ended. Goodbye!");
}

string ReadDescription()
{
    while (true)
    {
        Console.Write("Description: ");
        string? description = Console.ReadLine();

        if (description is null)
        {
            throw new EndOfStreamException();
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            return description.Trim();
        }

        Console.WriteLine("Description cannot be empty.");
    }
}

decimal ReadPositiveAmount()
{
    while (true)
    {
        Console.Write("Amount: ");
        string? input = Console.ReadLine();

        if (input is null)
        {
            throw new EndOfStreamException();
        }

        if (decimal.TryParse(
                input,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal amount)
            && amount > 0)
        {
            return amount;
        }

        Console.WriteLine("Please enter a valid amount greater than zero.");
    }
}

DateOnly ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (input is null)
        {
            throw new EndOfStreamException();
        }

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
            "Invalid date. Please use the format TT.MM.JJJJ, for example 23.10.2026."
        );
    }
}

void DisplayTransactions(List<Transaction> transactions)
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
            $"{transaction.Amount.ToString("C", CultureInfo.GetCultureInfo("de-DE"))}"
        );
    }
}