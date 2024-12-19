using System;
using System.Collections.Generic;
using System.Text;

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
            MurmurHash,
            FNV1aHash,
            PolynomialRollingHash,
            JenkinsOneAtATimeHash
        }

        private readonly HashMethod hashMethod;
        private readonly Random random = new Random(); // Для случайности

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
            switch (hashMethod)
            {
                case HashMethod.Division:
                    return HashDivision(key);
                case HashMethod.Multiplication:
                    return HashMultiplication(key);
                case HashMethod.MurmurHash:
                    return HashMurmur(key);
                case HashMethod.FNV1aHash:
                    return HashFNV1a(key);
                case HashMethod.PolynomialRollingHash:
                    return HashPolynomialRolling(key);
                case HashMethod.JenkinsOneAtATimeHash:
                    return HashJenkinsOneAtATime(key);
                default:
                    throw new InvalidOperationException("Неизвестный метод хеширования");
            }
        }

        // Метод деления
        private int HashDivision(TKey key)
        {
            if (key == null)
                return 0;

            // Хеш числа - само число
            if (key is int intKey)
                return Math.Abs(intKey) % TableSize;

            // Хеш строки - длина строки
            if (key is string str)
                return Math.Abs(str.Length) % TableSize;

            // Для других типов используем GetHashCode()
            return Math.Abs(key.GetHashCode()) % TableSize;
        }

        // Метод умножения
        private int HashMultiplication(TKey key)
        {
            if (key == null)
                return 0;

            // Хеш числа - само число
            if (key is int intKey)
            {
                double I = 0.6180339887; // Константа: 0 < A < 1
                return (int)(TableSize * ((intKey * I) % 1));
            }

            // Хеш строки - длина строки
            if (key is string str)
            {
                double S = 0.6180339887; // Константа: 0 < A < 1
                return (int)(TableSize * ((str.Length * S) % 1));
            }

            // Для других типов используем GetHashCode()
            int hash = key.GetHashCode();
            double O = 0.6180339887; // Константа: 0 < A < 1
            return (int)(TableSize * ((hash * O) % 1));
        }

        // MurmurHash
        private int HashMurmur(TKey key)
        {
            if (key == null)
                return 0;

            byte[] data = Encoding.UTF8.GetBytes(key.ToString());
            uint seed = (uint)random.Next(1, int.MaxValue); // Случайное семя
            uint hash = MurmurHash2(data, seed);
            return (int)(hash % TableSize);
        }

        private uint MurmurHash2(byte[] data, uint seed)
        {
            const uint m = 0x5bd1e995;
            const int r = 24;

            uint h = seed ^ (uint)data.Length;

            int index = 0;
            while (index + 4 <= data.Length)
            {
                uint k = BitConverter.ToUInt32(data, index);
                k *= m;
                k ^= k >> r;
                k *= m;
                h *= m;
                h ^= k;
                index += 4;
            }

            switch (data.Length - index)
            {
                case 3:
                    h ^= (uint)data[index + 2] << 16;
                    goto case 2;
                case 2:
                    h ^= (uint)data[index + 1] << 8;
                    goto case 1;
                case 1:
                    h ^= data[index];
                    h *= m;
                    break;
            }

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        }

        // FNV-1a
        private int HashFNV1a(TKey key)
        {
            if (key == null)
                return 0;

            byte[] data = Encoding.UTF8.GetBytes(key.ToString());
            uint seed = (uint)random.Next(1, int.MaxValue); // Случайное семя
            const uint offsetBasis = 2166136261;
            const uint prime = 16777619;

            uint hash = offsetBasis ^ seed;
            foreach (byte b in data)
            {
                hash ^= b;
                hash *= prime;
            }

            return (int)(hash % TableSize);
        }

        // Polynomial Rolling Hash
        private int HashPolynomialRolling(TKey key)
        {
            if (key == null)
                return 0;

            string str = key.ToString();
            const int p = 31; // Простое число
            const int mod = 1000000007; // Большое простое число
            long hash = 0;
            long p_pow = 1;

            foreach (char c in str)
            {
                hash = (hash + (c - 'a' + 1) * p_pow) % mod;
                p_pow = (p_pow * p) % mod;
            }

            // Убедимся, что результат положительный и находится в диапазоне [0, TableSize - 1]
            return (int)((hash + mod) % TableSize);
        }

        // Jenkins One-at-a-Time Hash
        private int HashJenkinsOneAtATime(TKey key)
        {
            if (key == null)
                return 0;

            byte[] data = Encoding.UTF8.GetBytes(key.ToString());
            uint hash = 0;

            foreach (byte b in data)
            {
                hash += b;
                hash += hash << 10;
                hash ^= hash >> 6;
            }

            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;

            return (int)(hash % TableSize);
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