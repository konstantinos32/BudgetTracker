namespace BudgetTracker.Models;

public class TransactionReport
{
    public List<Transaction> Transactions {get; set;} = new();

    public decimal TotalIncome{get; set;}

    public decimal TotalExpenses {get; set;}
    
    public decimal Balance => TotalIncome - TotalExpenses;
}