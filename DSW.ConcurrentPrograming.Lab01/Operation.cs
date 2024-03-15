namespace DSW.ConcurrentPrograming.Lab01;

public class Operation
{
    public string Id { get; }
    public OperationType OperationType { get; }
    public BankAccount? From { get; }
    public BankAccount? To { get; }
    public decimal Amount { get; }

    public bool SafeMode { get; }
    private Operation(OperationType operationType, decimal amount, BankAccount? from = null, BankAccount? to = null, bool safeMode = true)
    {
        OperationType = operationType;
        Amount = amount;
        From = from;
        To = to;
        SafeMode = safeMode;
        
        Id = $"{OperationType.ToString()}-{Guid.NewGuid().ToString()[..5]}";
    }

    public override string ToString()
    {
        return OperationType switch
        {
            OperationType.Withdraw => $"{From!.Id} ---> {Amount:C}",
            OperationType.Deposit => $"{Amount:C} ---> {To!.Id}",
            OperationType.Transfer => $"{From!.Id} ---{Amount:C}---> {To!.Id}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static Operation Withdraw(BankAccount from, decimal amount, bool safeMode = true) =>
        new Operation(OperationType.Withdraw, amount, from: from, safeMode: safeMode);

    public static Operation Deposit(BankAccount to, decimal amount, bool safeMode = true) =>
        new Operation(OperationType.Deposit, amount, to: to, safeMode: safeMode);

    public static Operation Transfer(BankAccount from, BankAccount to, decimal amount, bool safeMode = true) =>
        new Operation(OperationType.Transfer, amount, from: from, to: to, safeMode: safeMode);
}