public static class Program
{
    private static class Message
    {
        public const string TryAgain = "Пожалуйста, попробуйте еще раз";
        
        public static readonly string[] Operations = [
            "Создать массив",
            "Напечатать массив",
            "Удалить элемент из массива",
            "Добавить элементы в массив",
            "Циклически сдвинуть массив влево",
            "Найти элемент в массиве",
            "Отсортировать массив",
            "Найти элемент с помощью бинарного поиска",
            "Закончить работу программы"
        ];
        public const string OperationsChoiceLabel = "Какую операцию вы хотите провести?";

        public static readonly string[] ArrayCreationMethods = ["С клавиатуры", "Случайно"];
        public const string ArrayCreationLabel = "Каким методом вы хотие создать массив?";
        public const string ArrayEmpty = "Пустой массив";
    }

    private static int Mod(int a, int m) => (a % m + m) % m;
    private static int ChooseOperation(int active, string[] variants, string label)
    {
        var key = ConsoleKey.None;
        active--;

        while (key != ConsoleKey.Enter)
        {
            Console.Clear();
            Console.WriteLine(label);
            
            for (var i = 0; i < variants.Length; i++)
            {
                Console.BackgroundColor = i == active ? ConsoleColor.Cyan : ConsoleColor.Black;
                Console.ForegroundColor = i == active ? ConsoleColor.Black : ConsoleColor.White;

                Console.WriteLine(variants[i]);
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            key = Console.ReadKey().Key;
            
            active += key switch
            {
                ConsoleKey.UpArrow => -1,
                ConsoleKey.DownArrow => 1,
                _ => 0,
            };

            active = Mod(active, variants.Length);
        }

        return ++active;
    }
    
    public static void Main(string[] args)
    {
        int[] array = [];
        var isRunning = true;
        
        int operationId = 1;
        
        while (isRunning)
        {
            operationId = ChooseOperation(operationId, Message.Operations, Message.OperationsChoiceLabel);
            Console.WriteLine();

            isRunning = operationId switch
            {
                1 => CreateArray(ref array),
                2 => PrintArray(ref array),
                3 => RemoveElement(ref array),
                4 => AddElements(ref array),
                5 => ShiftLeft(ref array),
                6 => FindElement(ref array),
                7 => Sort(ref array),
                8 => FindElementBinary(ref array),
                9 => false,
                _ => true,
            };

            if (!isRunning)
                break;

            Console.WriteLine();
            Console.Write("Для продолжения нажмите любую клавишу");
            Console.ReadKey();
        }
    }

    private static bool FindElementBinary(ref int[] array)
    {
        if (!IsSorted(ref array))
        {
            Console.WriteLine("Бинарный поиск невозможен в неотсортированном массиве! Прекращаю выполнение операции");
            return true;
        }
        
        var target = Input.ReadInteger("Введите искомый элемент: ");

        if (array.Length == 0)
        {
            Console.WriteLine("В массиве нет искомого элемента");
            return true;
        }

        int l = 0;
        int r = array.Length - 1;

        int count = 0;

        while (l < r)
        {
            int m = l + (r - l) / 2;
            count++;

            if (array[m] == target)
            {
                l = r = m;
            }
            else if (array[m] < target)
            {
                l = m + 1;
            }
            else
            {
                r = m - 1;
            }
        }

        Console.WriteLine(array[l] != target
            ? "В массиве нет искомого элемента"
            : $"Найден элемент {target} в позиции {l + 1}. Для этого потребовалось {count} сравнений");

        return true;
    }

    private static bool IsSorted(ref int[] array)
    {
        if (array.Length < 2)
            return true;
        
        for (int i = 0; i < array.Length - 1; i++)
        {
            if (array[i] > array[i + 1])
                return false;
        }

        return true;
    }

    private static bool Sort(ref int[] array)
    {
        for (var i = 0; i < array.Length; i++)
        {
            var mini = i;
            var min = array[i];

            for (var j = i + 1; j < array.Length; j++)
            {
                if (array[j] >= min) continue;
                
                mini = j;
                min = array[j];
            }

            array[mini] = array[i];
            array[i] = min;
        }

        return true;
    }

    private static bool FindElement(ref int[] array)
    {
        var target = Input.ReadInteger("Введите искомый элемент: ");

        if (array.Length == 0)
        {
            Console.WriteLine("В массиве нет искомого элемента");
            return true;
        }

        var i = 0;
        while (i < array.Length && array[i] != target)
            i++;

        Console.WriteLine(i == array.Length
            ? "В массиве нет искомого элемента"
            : $"Найден элемент {target} в позиции {i + 1}. Для этого потребовалось {i + 1} сравнений");

        return true;
    }

    private static bool ShiftLeft(ref int[] array)
    {
        if (array.Length == 0)
        {
            Console.WriteLine($"{Message.ArrayEmpty}. Недостаточно элементов для сдвига. Прекращаю выполнение операции.");
            return true;
        }

        var m = Input.ReadInteger("Введите сдвиг: ");
        m = Mod(m, array.Length);

        var storage = new int[m];
        Copy(array.AsSpan(0, m), storage.AsSpan());
        Copy(array.AsSpan(m), array.AsSpan(0, array.Length - m));
        Copy(storage.AsSpan(), array.AsSpan(array.Length - m));

        return true;
    }

    private static bool AddElements(ref int[] array)
    {
        var count = Input.ReadLength("Введите количество добавляемых элементов: ");
        var position = Input.ReadIntegerRange("Введите позицию, в которую необходимо вставить элементы: ", 1,
            array.Length + 1,
            $"Позиция должна быть в пределах длины массива или на один больше, если вы хотите вставить в конец [1, {array.Length + 1}]. {Message.TryAgain}") - 1;

        var result = new int[array.Length + count];
        
        Copy(array.AsSpan(0, position), result.AsSpan(0, position));
        InitArrayKeyboard(result.AsSpan(position, count));
        Copy(array.AsSpan(position), result.AsSpan(position + count));

        array = result;

        return true;
    }

    private static bool RemoveElement(ref int[] array)
    {
        if (array.Length == 0)
        {
            Console.WriteLine($"{Message.ArrayEmpty}. Удаление невозможно. Прекращаю выполнение операции");
            return true;
        }
        
        var indexToRemove = Input.ReadIntegerRange("Введите номер удаляемого элемента: ", 1, array.Length,
            $"Вводимый элемент должен быть в промежутке от 1 до {array.Length}. {Message.TryAgain}") - 1;

        var result = new int[array.Length - 1];
        Copy(array.AsSpan(0, indexToRemove), result.AsSpan(0, indexToRemove));
        Copy(array.AsSpan(indexToRemove + 1), result.AsSpan(indexToRemove));

        array = result;
        return true;
    }

    private static void Copy(Span<int> from, Span<int> to)
    {
        if (from.Length != to.Length)
            throw new ArgumentException("Срезы должны быть одинаковой длины");

        for (int i = 0; i < from.Length; i++)
        {
            to[i] = from[i];
        }
    }

    private static bool PrintArray(ref int[] array)
    {
        if (array.Length == 0)
        {
            Console.WriteLine(Message.ArrayEmpty);
            return true;
        }
        
        foreach (var element in array)
        {
            Console.Write($"{element} ");
        }

        Console.WriteLine();
        return true;
    }
    
    private static bool CreateArray(ref int[] array)
    {
        var creationMethod = ChooseOperation(1, Message.ArrayCreationMethods, Message.ArrayCreationLabel);
        var arrayLength = Input.ReadLength("Введите длину создаваемого массива: ");

        array = new int[arrayLength];

        switch (creationMethod)
        {
            case 1:
                InitArrayKeyboard(array.AsSpan());
                break;
            case 2:
                InitArrayRandom(array.AsSpan());
                break;
        }

        return true;
    }

    private static void InitArrayRandom(Span<int> array)
    {
        var min = Input.ReadInteger("Введите нижнюю границу генерации: ");
        var max = Input.ReadIntegerGe("Введите верхнюю границу генерации: ", min);

        var random = new Random();
        
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = random.Next(min, max);
        }
    }

    private static void InitArrayKeyboard(Span<int> array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Input.ReadInteger($"Введите элемент №{i + 1}: ");
        }
    }

    private static class Input
    {
        public static int ReadInteger(string label)
        {
            int result;
            bool isConvert;

            do
            {
                Console.Write(label);
                isConvert = int.TryParse(Console.ReadLine(), out result);

                if (!isConvert) Console.WriteLine($"Вы ввели не целое число. {Message.TryAgain}");
            } while (!isConvert);

            return result;
        }

        public static int ReadIntegerGe(string label, int min) => ReadIntegerRange(label, min, int.MaxValue,
            $"Число должно быть больше или равно {min}. {Message.TryAgain}");

        public static int ReadLength(string label) => ReadIntegerRange(label, 1, int.MaxValue,
                $"Длина должна быть натуральным числом. {Message.TryAgain}");

        public static int ReadIntegerRange(string label, int from, int to, string onOverflow)
        {
            int result;

            do
            {
                result = ReadInteger(label);

                if (result < from || result > to) Console.WriteLine(onOverflow);
            } while (result < from || result > to);

            return result;
        }
    }
}
