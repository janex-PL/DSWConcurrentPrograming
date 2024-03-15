namespace DSW.ConcurrentPrograming.Lab01;

public class OperationProducer
{
    public readonly string Id = $"Producer_{Guid.NewGuid().ToString()[..5]}";
    private readonly Buffer _buffer;
    private readonly BankAccount[] _accounts;

    public OperationProducer(Buffer buffer, BankAccount[] accounts)
    {
        if (accounts.Length < 2)
            throw new ArgumentException("At least two accounts are required", nameof(accounts));
        
        _buffer = buffer;
        _accounts = accounts;
    }

    public void StartProducing()
    {
        while (true)
        {
            var operation = GenerateOperation();
            _buffer.Add(operation);
            LogMessage($"Enqueued operation {operation.Id}");
        }
    }

    private Operation GenerateOperation()
    {
        var operationType = (OperationType) Random.Shared.Next(0, 3);
        var amount = Random.Shared.Next(1, 1000);
        var accounts = Random.Shared.GetItems(_accounts, 2);
        
        var result = operationType switch
        {
            OperationType.Withdraw => Operation.Withdraw(accounts[0], amount),
            OperationType.Deposit => Operation.Deposit(accounts[0], amount),
            OperationType.Transfer => Operation.Transfer(accounts[0], accounts[1], amount),
            _ => throw new ArgumentOutOfRangeException()
        };
        LogMessage($"Generated operation {result.Id} - {result}");
        return result;
    }
    
    private void LogMessage(string message) => Console.WriteLine($"[{Id}] {message}");

}