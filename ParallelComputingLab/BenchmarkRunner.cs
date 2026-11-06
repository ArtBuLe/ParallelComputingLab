using System.Diagnostics;

namespace ParallelComputingLab;

public static class BenchmarkRunner
{
    // TODO 3: Измените количество измерений с 5 на 10.
    private const int MeasurementCount = 10;

    public static void Run(int[] numbers)
    {
        // ... (оставьте ваш существующий метод Run без изменений) ...
    }

    public static void CompareParallelism(int[] numbers)
    {
        // TODO 2: Добавьте степень параллелизма 3.
        int[] degrees = { 1, 2, 3, 4, -1 };

        Console.WriteLine();
        Console.WriteLine($"Размер массива: {numbers.Length:N0}");
        // TODO 4: Добавлен столбец "Ускорение" в заголовок
        Console.WriteLine("Степень параллелизма | Время, мс | Ускорение (x)");
        Console.WriteLine("-----------------------------------------------");

        // Прогрев и получение эталонного результата
        var baselineResult = Calculator.CalculateSequential(numbers);
        foreach (int degree in degrees)
        {
            Calculator.CalculateParallel(numbers, degree);
        }

        // Измеряем последовательное время для корректного расчета ускорения
        var seqTimes = new List<double>();
        for (int i = 0; i < MeasurementCount; i++)
        {
            seqTimes.Add(Measure(() => Calculator.CalculateSequential(numbers)).ElapsedMilliseconds);
        }
        double medianSeqTime = Median(seqTimes);

        foreach (int degree in degrees)
        {
            var times = new List<double>();

            for (int i = 0; i < MeasurementCount; i++)
            {
                var measurement = Measure(() => Calculator.CalculateParallel(numbers, degree));
                times.Add(measurement.ElapsedMilliseconds);

                // TODO 9: Проверка совпадения результата для каждого измеряемого варианта.
                if (measurement.Result != baselineResult)
                {
                    throw new InvalidOperationException(
                        $"Ошибка: результат для Degree={degree} не совпадает с последовательным!");
                }
            }

            double medianParTime = Median(times);

            // TODO 4: Вычисление ускорения (Sequential Time / Parallel Time)
            double speedup = medianSeqTime / medianParTime;

            string degreeStr = degree == -1 ? "Default (-1)" : degree.ToString();
            Console.WriteLine($"{degreeStr,20} | {medianParTime,10:F3} | {speedup,14:F2}x");
        }

        /*
         * =====================================================================
         * ОТВЕТЫ НА ВОПРОСЫ (TODO 5, 6, 7, 8, 10):
         * 
         * TODO 5: На каком размере массива параллельная обработка стала быстрее?
         * Обычно это происходит на размере от 100_000 до 1_000_000 элементов.
         * На 10_000 накладные расходы на создание потоков превышают выгоду.
         * 
         * TODO 6: Какое значение MaxDegreeOfParallelism показало лучший результат?
         * Зависит от вашего CPU. На 4-ядерном/8-поточном процессоре лучшим 
         * часто является 4 или Default (-1), так как он использует все логические ядра.
         * 
         * TODO 7: Повторите эксперимент после перезапуска программы.
         * Результаты могут незначительно отличаться (на 5-15%) из-за фоновых 
         * процессов ОС, состояния кэша процессора и работы сборщика мусора (GC).
         * 
         * TODO 8: Почему результат Degree = 4 может быть хуже Degree = 2?
         * 1. Накладные расходы на переключение контекста между большим числом потоков.
         * 2. Конкуренция за ресурсы (кэш-память L1/L2, шина памяти).
         * 3. Если физических ядер всего 2, а логических 4 (Hyper-Threading), 
         *    то 4 потока делят одни и те же физические исполнительные блоки, 
         *    что не дает двукратного прироста, но добавляет издержки синхронизации.
         * 
         * TODO 10: Всегда ли параллельная обработка быстрее последовательной?
         * НЕТ. Для малых объемов данных (например, 10_000 элементов) 
         * последовательный цикл всегда быстрее из-за отсутствия накладных 
         * расходов на планирование потоков и синхронизацию (Interlocked/lock).
         * =====================================================================
         */
    }

    public static (CalculationResult Result, double ElapsedMilliseconds)
        Measure(Func<CalculationResult> action)
    {
        var stopwatch = Stopwatch.StartNew();
        CalculationResult result = action();
        stopwatch.Stop();
        return (result, stopwatch.Elapsed.TotalMilliseconds);
    }

    private static double Median(List<double> values)
    {
        double[] sorted = values.OrderBy(x => x).ToArray();
        int middle = sorted.Length / 2;
        if (sorted.Length % 2 == 1)
        {
            return sorted[middle];
        }
        return (sorted[middle - 1] + sorted[middle]) / 2;
    }
}