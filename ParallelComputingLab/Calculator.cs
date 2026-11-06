namespace ParallelComputingLab;

public static class Calculator
{
    public static CalculationResult CalculateSequential(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        long sum = 0;
        int count = 0;
        int? max = null; // TODO 2

        foreach (int number in numbers)
        {
            // TODO 1. Измените условие отбора:
            // вместо чисел, делящихся на 7,
            // выбирайте числа, которые делятся на 13.
            if (number % 13 == 0)
            {
                sum += number;
                count++;

                // TODO 2. Добавьте вычисление максимального
                // подходящего элемента.
                if (max == null || number > max)
                {
                    max = number;
                }
            }
        }

        return new CalculationResult(sum, count, max);
    }

    public static CalculationResult CalculateParallel
        (
        int[] numbers,
        int maxDegreeOfParallelism = -1
        )
    {
        ArgumentNullException.ThrowIfNull(numbers);

        if (maxDegreeOfParallelism != -1 &&
            maxDegreeOfParallelism < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxDegreeOfParallelism));
        }

        long sum = 0;
        int count = 0;
        int? max = null;

        object maxLock = new();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };

        Parallel.For(
            0,
            numbers.Length,
            options,
            () => (LocalSum: 0L, LocalCount: 0, LocalMax: (int?)null),
            (i, state, local) =>
            {
                int number = numbers[i];

                // TODO 1. Измените условие отбора:
                // вместо чисел, делящихся на 7,
                // выбирайте числа, которые делятся на 13.
                if (number % 13 == 0)
                {
                    local.LocalSum += number;
                    local.LocalCount++;

                    // TODO 2. Добавьте вычисление максимального
                    // подходящего элемента.
                    if (max == null || number > max)
                    {
                        max = number;
                    }
                }

                return local;
            },
            local =>
            {
                Interlocked.Add(ref sum, local.LocalSum);
                Interlocked.Add(ref count, local.LocalCount);

                if (local.LocalMax != null)
                {
                    lock (maxLock)
                    {
                        if (max == null || local.LocalMax > max)
                        {
                            max = local.LocalMax;
                        }
                    }
                }
            });

        return new CalculationResult(sum, count, max);
    }

    // TODO 7:
    public static CalculationResult CalculateParallelWrong(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        long sum = 0;
        int count = 0;
        int? maxValue = null;
        object maxLock = new object();

        Parallel.For(
            0,
            numbers.Length,
            () => (LocalSum: 0L, LocalCount: 0, LocalMax: (int?)null),
            (i, state, local) =>
            {
                int number = numbers[i];

                if (number % 13 == 0)
                {
                    local.LocalSum += number;
                    local.LocalCount++;
                    if (local.LocalMax == null || number > local.LocalMax)
                    {
                        local.LocalMax = number;
                    }
                }

                return local;
            },
            local =>
            {
                Interlocked.Add(ref sum, local.LocalSum);
                Interlocked.Add(ref count, local.LocalCount);

                if (local.LocalMax != null)
                {
                    lock (maxLock)
                    {
                        if (maxValue == null || local.LocalMax > maxValue)
                        {
                            maxValue = local.LocalMax;
                        }
                    }
                }
            });

        return new CalculationResult(sum, count, maxValue);
    }
}