using FluentResults;

namespace DSW.ConcurrentPrograming.Lab01;

public class BankAccount(decimal balance)
{
    public string Id { get; } = $"Account_{Guid.NewGuid().ToString()[..5]}";
    public decimal Balance { get; private set; } = balance;
    private readonly object _lockObject = new();

    private Result Withdraw(decimal amount)
    {
        if (amount < 0)
            return Result.Fail($"Cannot withdraw negative amount ({amount})");
        if (amount > Balance)
            return Result.Fail($"Requested amount {amount} exceeds the balance {Balance}");
            
        Thread.Sleep(Random.Shared.Next(500, 2500));
        Balance -= amount;
        
        return Result.Ok();
    }
    
    private Result Deposit(decimal amount)
    {
        Thread.Sleep(Random.Shared.Next(500, 1500));
        
        Balance += amount;

        return Result.Ok();
    }
    
    public Result WithdrawUnsafe(decimal amount)
    {
        return Withdraw(amount);
    }

    public Result WithdrawSafe(decimal amount)
    {
        lock (_lockObject)
        {
            return Withdraw(amount);
        }
    }
    public Result DepositUnsafe(decimal amount)
    { 
       return Deposit(amount);
    }
    public Result DepositSafe(decimal amount)
    {
        lock (_lockObject)
        {
            return Deposit(amount);
        }
    }
}