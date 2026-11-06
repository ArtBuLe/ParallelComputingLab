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

        // TODO 5: Проверка null
        Console.WriteLine("\n--- Тест null ---");
        try
        {
            Calculator.CalculateSequential(null);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Ожидаемое исключение поймано: {ex.GetType().Name}");
            Console.WriteLine("Объяснение: Метод содержит ArgumentNullException.ThrowIfNull(numbers);");
            Console.WriteLine("Это стандартная и правильная практика валидации входных параметров в .NET,");
            Console.WriteLine("которая предотвращает падение с NullReferenceException глубже в коде.");
        }

        // TODO 6:
        Console.WriteLine("\n--- Демонстрация CalculateParallelWrong ---");
        int[] largeArray = DataGenerator.Generate(size: 5_000_000, seed: 42);

        var wrongResult = Calculator.CalculateParallelWrong(largeArray);
        var correctResult = Calculator.CalculateParallel(largeArray);

        Console.WriteLine($"Правильный результат: Сумма = {correctResult.Sum}, Кол-во = {correctResult.Count}");
        Console.WriteLine($"Неправильный результат:      Сумма = {wrongResult.Sum}, Кол-во = {wrongResult.Count}");
        Console.WriteLine($"Совпадение: {wrongResult.Sum == correctResult.Sum && wrongResult.Count == correctResult.Count}");

        // TODO 7:
        Console.WriteLine("\n--- Проверка исправленного CalculateParallelWrong ---");
        var fixedWrongResult = Calculator.CalculateParallelWrong(largeArray);
        Console.WriteLine($"Исправленный результат: Сумма = {fixedWrongResult.Sum}, Кол-во = {fixedWrongResult.Count}");
        Console.WriteLine($"Совпадение с правильным: {fixedWrongResult.Sum == correctResult.Sum && fixedWrongResult.Count == correctResult.Count} (всегда True)");

        Console.WriteLine("\nРабота завершена.");

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