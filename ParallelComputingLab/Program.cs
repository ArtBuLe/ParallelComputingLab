namespace ParallelComputingLab;

internal class Program
{
    static void Main()
    {
        Console.WriteLine("=== Parallel Computing Lab ===");

        // TODO 3: Проверка массива из одного элемента (подходящего и неподходящего)
        RunCustomTest("Один подходящий элемент (13)", new[] { 13 });
        RunCustomTest("Один неподходящий элемент (14)", new[] { 14 });

        // TODO 4: Проверка пустого массива
        RunCustomTest("Пустой массив", new int[0]);

        int[] numbers =
        DataGenerator.Generate(
            size: 1_000_000,
            seed: 12345
            );
        BenchmarkRunner.Run(numbers);

        Console.WriteLine();
        Console.WriteLine("Работа завершена.");
    }

    static void RunCustomTest(string testName, int[] data)
    {
        Console.WriteLine($"Тест: {testName}");
        var seq = Calculator.CalculateSequential(data);
        var par = Calculator.CalculateParallel(data);

        Console.WriteLine($"  Последовательно: Сумма={seq.Sum}, Кол-во={seq.Count}, Макс={seq.Max}");
        Console.WriteLine($"  Параллельно:     Сумма={par.Sum}, Кол-во={par.Count}, Макс={par.Max}");
        Console.WriteLine($"  Результаты совпадают: {seq == par}\n");
    }
}