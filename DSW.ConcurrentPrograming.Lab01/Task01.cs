namespace DSW.ConcurrentPrograming.Lab01;

public class Task01
{
    public static void Run()
    {
        Console.Clear();
        
        var account = new BankAccount(100);

        var consumer = new OperationConsumer();
        
        var threads = Enumerable.Repeat(0, 5).Select(_ =>
                new Thread(() => consumer.PerformOperation(Operation.Withdraw(account, 50, safeMode: false))))
            .ToList();

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());

        Console.WriteLine($"Final account balance after unsafe withdrawal: {account.Balance}");

        Console.WriteLine(new string('#',20));

        account = new BankAccount(100);

        threads = Enumerable.Repeat(0, 5).Select(_ =>
                new Thread(() => consumer.PerformOperation(Operation.Withdraw(account, 50, safeMode: true))))
            .ToList();

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());

        Console.WriteLine($"Final account balance after safe withdrawal: {account.Balance}");
    }
}