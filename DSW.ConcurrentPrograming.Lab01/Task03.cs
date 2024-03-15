namespace DSW.ConcurrentPrograming.Lab01;

public class Task03
{
    public static void Run()
    {
        Console.Clear();

        var bufferSize = ConsoleExtensions.ReadInt("Input buffer size: ", x => x > 0);
        var buffer = new Buffer(bufferSize);

        var bankAccountsSize = ConsoleExtensions.ReadInt("Input bank accounts size: ", x => x > 1);
        var bankAccounts = Enumerable.Range(1, bankAccountsSize).Select(x => new BankAccount(x * 100)).ToArray();
        
        var threads = new List<Thread>();
        
        var producerThreadsCount = ConsoleExtensions.ReadInt("Input producer threads count: ", x => x > 0);
        var consumerThreadsCount = ConsoleExtensions.ReadInt("Input consumer threads count: ", x => x > 0);
        
        for (int i = 0; i < producerThreadsCount; i++)
        {
            threads.Add(new Thread(() =>
            {
                var producer = new OperationProducer(buffer, bankAccounts);
                producer.StartProducing();
            }));
        }
        for (int i = 0; i < consumerThreadsCount; i++)
        {
            threads.Add(new Thread(() =>
            {
                var consumer = new OperationConsumer(buffer);
                consumer.StartConsuming();
            }));
        }
        
        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());
    }
}

public static class ConsoleExtensions
{
public static int ReadInt(string message, Func<int, bool> func)
{
    var input = "";
    while (int.TryParse(input, out var result) == false && !func(result))
    {
        Console.Write(message);
        input = Console.ReadLine();
    }

    return int.Parse(input!);
}
}