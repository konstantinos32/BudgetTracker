# BudgetTracker

A console-based personal budget tracker built with C# and .NET 10. The application allows users to record income and expenses, remove transactions, and review financial summaries for a selected date range.

## Features

- Add income and expense transactions.
- Validate descriptions and amounts.
- Store transactions in daily JSON files.
- Remove transactions by their unique ID.
- Generate reports for a selected date range.
- Calculate total income, total expenses, and balance.
- Filter transactions by type and date range.
- Log newly added transactions using an event-driven `LoggerService`.
- Handle invalid input, malformed JSON, and file-access errors.

## Technologies

- C#
- .NET 10
- LINQ
- `System.Text.Json`
- JSON file storage
- Events and event handlers

## Project structure

```text
BudgetTracker/
├── Events/
│   └── TransactionAddedEventArgs.cs
├── Models/
│   ├── Transaction.cs
│   ├── TransactionReport.cs
│   └── TransactionType.cs
├── Services/
│   ├── LoggerService.cs
│   ├── StorageService.cs
│   └── TransactionService.cs
├── Program.cs
├── BudgetTracker.csproj
└── README.md
```

## Requirements

- .NET 10 SDK
- A terminal or code editor such as Visual Studio Code

## Getting started

Clone the repository and navigate to the project directory:

```bash
git clone https://github.com/konstantinos32/BudgetTracker
cd BudgetTracker
```

Build the application:

```bash
dotnet build
```

Run it:

```bash
dotnet run
```

The application creates its `data/` and `logs/` directories when needed.

## Using the application

Choose an option from the console menu:

1. Add income.
2. Add an expense.
3. Remove a transaction using its ID.
4. View a report for a date range.
5. View income or expenses for a date range.
0. Exit.

Dates for reports must use the format `TT.MM.JJJJ`, for example `23.10.2026`.

Enter amounts using the number format accepted by your current system locale. Amounts must be greater than zero, and descriptions cannot be empty.

## Data storage

Transactions are stored in JSON files under `data/`, with one file per calendar day. Filenames use the `yyyy-MM-dd.json` format.

Each transaction contains:

- `Id`: unique transaction identifier.
- `Timestamp`: date and time the transaction was created.
- `Type`: `Income` or `Expense`.
- `Description`: transaction description.
- `Amount`: positive monetary amount.

The `logs/transactions.log` file records newly added transactions.

## Financial calculations

Reports calculate:

- **Total income:** sum of income transactions in the selected period.
- **Total expenses:** sum of expense transactions in the selected period.
- **Balance:** total income minus total expenses.

The application uses LINQ to filter transactions and calculate totals.

## Error handling

The application validates user input and reports errors when transaction data cannot be read or files cannot be accessed. Malformed JSON is reported as invalid transaction data rather than silently treated as an empty transaction list.