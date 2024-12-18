using System;
using System.Diagnostics;
using HashTables;

namespace HashTableDemo
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите задачу:");
                Console.WriteLine("1 - Хеш-таблица с цепочками");
                Console.WriteLine("2 - Хеш-таблица с открытой адресацией");
                Console.WriteLine("3 - Выход");
                Console.Write("Введите ваш выбор: ");

                if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 3)
                {
                    Console.WriteLine("Некорректный выбор. Нажмите любую клавишу, чтобы попробовать снова...");
                    Console.ReadKey();
                    continue;
                }

                if (choice == 3)
                {
                    Console.WriteLine("Выход из программы...");
                    break;
                }

                switch (choice)
                {
                    case 1:
                        RunTask1();
                        break;
                    case 2:
                        RunTask2();
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу, чтобы вернуться в меню...");
                Console.ReadKey();
            }
        }

        private static void RunTask1()
        {
            const int NumKeys = 100000;
            Random random = new Random();
            var keys = new int[NumKeys];
            for (int i = 0; i < NumKeys; i++)
                keys[i] = random.Next(int.MaxValue);

            Console.Clear();
            Console.WriteLine("\n--- Тестирование хеш-таблицы с цепочками ---\n");

            foreach (HashTableChaining<int, string>.HashMethod method in Enum.GetValues(typeof(HashTableChaining<int, string>.HashMethod)))
            {
                Console.WriteLine($"Используем метод хеширования: {method}");
                var hashTable = new HashTableChaining<int, string>(method);
                Stopwatch stopwatch = Stopwatch.StartNew();

                foreach (int key in keys)
                    hashTable.Insert(key, $"Value {key}");

                stopwatch.Stop();
                var (minChain, maxChain) = hashTable.GetChainLengths();
                double loadFactor = hashTable.GetLoadFactor();

                Console.WriteLine($"Время вставки: {stopwatch.ElapsedMilliseconds} мс");
                Console.WriteLine($"Коэффициент заполнения: {loadFactor:F4}");
                Console.WriteLine($"Самая короткая цепочка: {minChain}, Самая длинная цепочка: {maxChain}\n");
            }
        }

        private static void RunTask2()
        {
            const int NumKeys = 10000;
            Random random = new Random();
            var keys = new int[NumKeys];
            for (int i = 0; i < NumKeys; i++)
                keys[i] = random.Next(int.MaxValue);

            Console.Clear();
            Console.WriteLine("\n--- Тестирование хеш-таблицы с открытой адресацией ---\n");

            foreach (HashTableOpenAddressing<int, string>.CollisionResolution method in Enum.GetValues(typeof(HashTableOpenAddressing<int, string>.CollisionResolution)))
            {
                Console.WriteLine($"Используем метод разрешения коллизий: {method}");
                var hashTable = new HashTableOpenAddressing<int, string>(method);
                Stopwatch stopwatch = Stopwatch.StartNew();

                foreach (int key in keys)
                    hashTable.Insert(key, $"Value {key}");

                stopwatch.Stop();
                int longestCluster = hashTable.GetLongestCluster();

                Console.WriteLine($"Время вставки: {stopwatch.ElapsedMilliseconds} мс");
                Console.WriteLine($"Самый длинный кластер: {longestCluster}\n");
            }
        }
    }
}