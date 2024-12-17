namespace HashTables;

// File: ProgramTaskContinuation.cs
using System;
using System.Diagnostics;

// Дополнение к Program для Задач 1 и 2
public partial class Program
{
    private static void RunTask1()
    {
        const int NumKeys = 100000;
        Random random = new Random();
        int[] keys = new int[NumKeys];
        for (int i = 0; i < NumKeys; i++)
            keys[i] = random.Next(int.MaxValue);

        Console.WriteLine("\n--- Тестирование хеш-таблицы с цепочками (Задача 1) ---\n");

        foreach (HashTableChaining.HashMethod method in Enum.GetValues(typeof(HashTableChaining.HashMethod)))
        {
            Console.WriteLine($"Используем метод хеширования: {method}");
            HashTableChaining hashTable = new HashTableChaining(method);
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (int key in keys)
                hashTable.Insert(key);

            stopwatch.Stop();
            var (minChain, maxChain) = hashTable.GetChainLengths();
            double loadFactor = hashTable.GetLoadFactor();

            Console.WriteLine($"Время вставки: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine($"Коэффициент заполнения: {loadFactor:F4}");
            Console.WriteLine($"Самая короткая цепочка: {minChain}, Самая длинная цепочка: {maxChain}\n");

            // Дополнительная демонстрация операций
            int testKey = keys[random.Next(NumKeys)];
            Console.WriteLine($"Поиск ключа {testKey}: {(hashTable.Search(testKey) ? "найден" : "не найден")}");
            Console.WriteLine($"Удаление ключа {testKey}: {(hashTable.Delete(testKey) ? "успешно" : "не найден")}");
            Console.WriteLine($"Повторный поиск ключа {testKey}: {(hashTable.Search(testKey) ? "найден" : "не найден")}\n");
        }
    }

    private static void RunTask2()
    {
        const int NumKeys = 10000;
        Random random = new Random();
        int[] keys = new int[NumKeys];
        for (int i = 0; i < NumKeys; i++)
            keys[i] = random.Next(int.MaxValue);

        Console.WriteLine("\n--- Тестирование хеш-таблицы с открытой адресацией (Задача 2) ---\n");

        foreach (HashTableOpenAddressing.CollisionResolution method in Enum.GetValues(typeof(HashTableOpenAddressing.CollisionResolution)))
        {
            Console.WriteLine($"Используем метод разрешения коллизий: {method}");
            HashTableOpenAddressing hashTable = new HashTableOpenAddressing(method);
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (int key in keys)
                hashTable.Insert(key);

            stopwatch.Stop();
            int longestCluster = hashTable.GetLongestCluster();

            Console.WriteLine($"Время вставки: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine($"Самый длинный кластер: {longestCluster}\n");

            // Дополнительная демонстрация операций
            int testKey = keys[random.Next(NumKeys)];
            Console.WriteLine($"Поиск ключа {testKey}: {(hashTable.Search(testKey) ? "найден" : "не найден")}");
            Console.WriteLine($"Удаление ключа {testKey}: {(hashTable.Delete(testKey) ? "успешно" : "не найден")}");
            Console.WriteLine($"Повторный поиск ключа {testKey}: {(hashTable.Search(testKey) ? "найден" : "не найден")}\n");
        }
    }

    private static void CompareTables()
    {
        Console.WriteLine("\n--- Сравнение реализаций хеш-таблиц ---\n");

        const int NumKeys = 10000;
        Random random = new Random();
        int[] keys = new int[NumKeys];
        for (int i = 0; i < NumKeys; i++)
            keys[i] = random.Next(int.MaxValue);

        // Цепочки
        HashTableChaining chainingTable = new HashTableChaining(HashTableChaining.HashMethod.Division);
        Stopwatch chainingTimer = Stopwatch.StartNew();
        foreach (int key in keys)
            chainingTable.Insert(key);
        chainingTimer.Stop();
        var (minChain, maxChain) = chainingTable.GetChainLengths();
        double loadFactor = chainingTable.GetLoadFactor();

        // Открытая адресация
        HashTableOpenAddressing openTable = new HashTableOpenAddressing(HashTableOpenAddressing.CollisionResolution.LinearProbing);
        Stopwatch openTimer = Stopwatch.StartNew();
        foreach (int key in keys)
            openTable.Insert(key);
        openTimer.Stop();
        int longestCluster = openTable.GetLongestCluster();

        // Результаты
        Console.WriteLine("Результаты для хеш-таблицы с цепочками:");
        Console.WriteLine($"Время вставки: {chainingTimer.ElapsedMilliseconds} мс");
        Console.WriteLine($"Коэффициент заполнения: {loadFactor:F4}");
        Console.WriteLine($"Самая короткая цепочка: {minChain}, Самая длинная цепочка: {maxChain}\n");

        Console.WriteLine("Результаты для хеш-таблицы с открытой адресацией (линейное исследование):");
        Console.WriteLine($"Время вставки: {openTimer.ElapsedMilliseconds} мс");
        Console.WriteLine($"Самый длинный кластер: {longestCluster}\n");
    }

    public static void Main()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("Выберите задачу:");
            Console.WriteLine("1 - Хеш-таблица с цепочками (Задача 1)");
            Console.WriteLine("2 - Хеш-таблица с открытой адресацией (Задача 2)");
            Console.WriteLine("3 - Сравнение реализаций хеш-таблиц");
            Console.WriteLine("4 - Выход");
            Console.Write("Введите ваш выбор: ");

            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    RunTask1();
                    break;
                case 2:
                    RunTask2();
                    break;
                case 3:
                    CompareTables();
                    break;
                case 4:
                    exit = true;
                    Console.WriteLine("Выход...");
                    break;
                default:
                    Console.WriteLine("Некорректный выбор.");
                    break;
            }
            
            if (!exit)
            {
                Console.WriteLine("Для выхода в меню нажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }
}