namespace DSW.ConcurrentPrograming.Lab01;

public class Task02
{
    public static void Run()
    {
        Console.WriteLine("Select run mode:");
        Console.WriteLine("\ts - safe mode");
        Console.WriteLine("\tu - unsafe mode (program blocks)");

        var input = ' ';
        while (input != 's' && input != 'u')
        {
            Console.Write("Input:");
            input = Console.ReadKey().KeyChar;
            Console.WriteLine();
        }

        var (bankAccountA, bankAccountB) = (new BankAccount(100),new BankAccount(200));

        var consumer = new OperationConsumer();

        List<Thread> threads =
        [
            new Thread(() =>
                consumer.PerformOperation(Operation.Transfer(bankAccountA, bankAccountB, 100, input == 's'))),
            new Thread(
                () => consumer.PerformOperation(Operation.Transfer(bankAccountB, bankAccountA, 50, input == 's'))),
        ];
        
        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());
    }
}