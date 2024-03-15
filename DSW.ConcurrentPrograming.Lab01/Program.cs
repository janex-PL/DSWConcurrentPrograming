// See https://aka.ms/new-console-template for more information

using DSW.ConcurrentPrograming.Lab01;


while (true)
{
    Console.Clear();
    Console.WriteLine("DSW - Programowanie Współbieżne");
    Console.WriteLine("Jan Kliszcz, Indeks: 48224");
    Console.WriteLine(new string('#', 20));
    Console.WriteLine("Select task: ");
    Console.WriteLine("\t1 - Race condition example");
    Console.WriteLine("\t2 - Deadlock example");
    Console.WriteLine("\t3 - Producer/Consumer example");
    Console.WriteLine("q - Exit");
    Console.Write("Input:");
    var input = Console.ReadKey().KeyChar;
    Console.WriteLine();
    if (input == 'q')
        break;

    switch (input)
    {
        case '1':
            Task01.Run();
            WaitForKey();
            break;
        case '2':
            Task02.Run();
            WaitForKey();
            break;
        case '3':
            Task03.Run();
            WaitForKey();
            break;
    }
}

void WaitForKey()
{
    Console.Write("Press any key to continue ...");
    Console.ReadKey();
}