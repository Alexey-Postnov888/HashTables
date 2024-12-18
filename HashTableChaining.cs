using System;
using System.Collections.Generic;

namespace HashTables
{
    public class HashTableChaining<TKey, TValue>
    {
        private const int TableSize = 1000; // Размер таблицы
        private List<(TKey Key, TValue Value)>[] table;

        public enum HashMethod
        {
            Division,
            Multiplication,
            XorConstantHash,
            RotateLeftHash,
            XorAddHash,
            PolynomialHash
        }

        private readonly HashMethod hashMethod;

        public HashTableChaining(HashMethod method)
        {
            table = new List<(TKey, TValue)>[TableSize];
            for (int i = 0; i < TableSize; i++)
                table[i] = new List<(TKey, TValue)>();

            hashMethod = method;
        }

        // Вставка или обновление пары ключ-значение
        public void Insert(TKey key, TValue value)
        {
            int index = GetHash(key);
            var chain = table[index];

            for (int i = 0; i < chain.Count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(chain[i].Key, key))
                {
                    chain[i] = (key, value); // Обновление значения
                    return;
                }
            }

            chain.Add((key, value)); // Добавление новой пары
        }

        // Поиск значения по ключу
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = GetHash(key);
            var chain = table[index];

            foreach (var pair in chain)
            {
                if (EqualityComparer<TKey>.Default.Equals(pair.Key, key))
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        // Удаление пары по ключу
        public bool Remove(TKey key)
        {
            int index = GetHash(key);
            var chain = table[index];

            for (int i = 0; i < chain.Count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(chain[i].Key, key))
                {
                    chain.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private int GetHash(TKey key)
        {
            int hash = key.GetHashCode();
            switch (hashMethod)
            {
                case HashMethod.Division:
                    return Math.Abs(hash) % TableSize;
                case HashMethod.Multiplication:
                    double A = 0.6180339887; // Константа: 0 < A < 1
                    return (int)(TableSize * ((hash * A) % 1));
                case HashMethod.XorConstantHash:
                    return Math.Abs((hash ^ 1234567) % TableSize);
                case HashMethod.RotateLeftHash:
                    return Math.Abs(((hash << 5) | (hash >> 27)) % TableSize);
                case HashMethod.XorAddHash:
                    return Math.Abs(((hash ^ (hash << 3)) + (hash ^ (hash >> 5))) % TableSize);
                case HashMethod.PolynomialHash:
                    return Math.Abs((hash * 31 + 17) % TableSize);
                default:
                    throw new InvalidOperationException("Неизвестный метод хеширования");
            }
        }

        // Статистика: минимальная и максимальная длина цепочек
        public (int minLength, int maxLength) GetChainLengths()
        {
            int min = int.MaxValue, max = 0;
            foreach (var chain in table)
            {
                int length = chain.Count;
                if (length > max) max = length;
                if (length < min) min = length;
            }
            return (min == int.MaxValue ? 0 : min, max);
        }

        // Коэффициент заполнения
        public double GetLoadFactor()
        {
            int nonEmptyChains = 0;
            foreach (var chain in table)
            {
                if (chain.Count > 0) nonEmptyChains++;
            }
            return (double)nonEmptyChains / TableSize;
        }
    }
}