using System;
using System.Collections.Generic;

namespace HashTables
{
    public class HashTableOpenAddressing<TKey, TValue>
    {
        private static int TableSize = 10000;
        private (TKey Key, TValue Value)?[] table;
        private int currentSize;

        public enum CollisionResolution
        {
            LinearProbing,
            QuadraticProbing,
            DoubleHashing,
            ModuloOffsetProbing,
            XorConstantProbing
        }

        private readonly CollisionResolution resolutionMethod;

        public HashTableOpenAddressing(CollisionResolution method)
        {
            table = new (TKey, TValue)?[TableSize];
            currentSize = 0;
            resolutionMethod = method;
        }

        // Вставка или обновление пары ключ-значение
        public void Insert(TKey key, TValue value)
        {
            if ((double)currentSize / TableSize >= 0.5)
                Rehash();

            int index = CustomHash(key);
            int i = 0;

            while (i < TableSize)
            {
                int probeIndex = GetProbeIndex(index, key, i);
                if (table[probeIndex] == null || EqualityComparer<TKey>.Default.Equals(table[probeIndex].Value.Key, key))
                {
                    table[probeIndex] = (key, value);
                    currentSize++;
                    return;
                }
                i++;
            }

            throw new InvalidOperationException("Не удалось вставить ключ: таблица заполнена или произошла ошибка");
        }

        // Поиск значения по ключу
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = CustomHash(key);
            int i = 0;

            while (i < TableSize)
            {
                int probeIndex = GetProbeIndex(index, key, i);
                if (table[probeIndex] == null)
                    break;

                if (EqualityComparer<TKey>.Default.Equals(table[probeIndex].Value.Key, key))
                {
                    value = table[probeIndex].Value.Value;
                    return true;
                }
                i++;
            }

            value = default;
            return false;
        }

        // Удаление пары по ключу
        public bool Remove(TKey key)
        {
            int index = CustomHash(key);
            int i = 0;

            while (i < TableSize)
            {
                int probeIndex = GetProbeIndex(index, key, i);
                if (table[probeIndex] == null)
                    break;

                if (EqualityComparer<TKey>.Default.Equals(table[probeIndex].Value.Key, key))
                {
                    table[probeIndex] = null;
                    currentSize--;
                    return true;
                }
                i++;
            }

            return false;
        }

        private void Rehash()
        {
            int newSize = GetNextPrime(TableSize * 2);
            var oldTable = table;

            table = new (TKey, TValue)?[newSize];
            TableSize = newSize;
            currentSize = 0;

            foreach (var pair in oldTable)
            {
                if (pair.HasValue)
                    Insert(pair.Value.Key, pair.Value.Value);
            }
        }

        private int GetNextPrime(int start)
        {
            while (!IsPrime(start)) start++;
            return start;
        }

        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            for (int i = 2; i * i <= number; i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        private int CustomHash(TKey key)
        {
            if (key == null)
                return 0;

            // Хеш для чисел
            if (key is int intKey)
                return Math.Abs(intKey) % TableSize;

            // Хеш для строк
            if (key is string str)
            {
                int hash = 0;
                foreach (char c in str)
                {
                    hash = (hash * 31) + c; // Полиномиальный хеш
                }
                return Math.Abs(hash) % TableSize;
            }

            // Для других типов используем длину объекта
            return Math.Abs(key.ToString().Length) % TableSize;
        }

        private int GetProbeIndex(int hash, TKey key, int i)
        {
            switch (resolutionMethod)
            {
                case CollisionResolution.LinearProbing:
                    return (hash + i) % TableSize;
                case CollisionResolution.QuadraticProbing:
                    return (hash + i * i) % TableSize;
                case CollisionResolution.DoubleHashing:
                    return (hash + i * SecondHash(key)) % TableSize;
                case CollisionResolution.ModuloOffsetProbing:
                    return (hash + i * (CustomHash(key) % 7 + 1)) % TableSize;
                case CollisionResolution.XorConstantProbing:
                    return (hash + i * ((CustomHash(key) ^ 12345) % TableSize)) % TableSize;
                default:
                    throw new InvalidOperationException("Неизвестный метод разрешения коллизий");
            }
        }

        private int SecondHash(TKey key)
        {
            return 7 - (CustomHash(key) % 7);
        }

        // Подсчет длины самого длинного кластера в таблице (для открытой адресации)
        public int GetLongestCluster()
        {
            int maxCluster = 0, currentCluster = 0;

            foreach (var cell in table)
            {
                if (cell != null) // Если ячейка занята
                    currentCluster++;
                else
                {
                    // Обновляем максимальный кластер, если текущий завершен
                    if (currentCluster > maxCluster)
                        maxCluster = currentCluster;
                    currentCluster = 0;
                }
            }

            // Проверяем последний кластер, если таблица не закончилась пустой ячейкой
            return Math.Max(maxCluster, currentCluster);
        }
    }
}