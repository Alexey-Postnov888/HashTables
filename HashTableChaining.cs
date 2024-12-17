namespace HashTables;

// Хеш-таблица с цепочками

public class HashTableChaining : IHashTable
{
    private const int TableSize = 1000; // Размер таблицы
    private List<int>[] table; // Массив списков для цепочек

    public enum HashMethod
    {
        Division,
        Multiplication,
        XorConstantHash,
        RotateLeftHash,
        XorAddHash,
        PolynomialHash
    }
    
    private HashMethod hashMethod;

    public HashTableChaining(HashMethod method)
    {
        table = new List<int>[TableSize];
        for (int i = 0; i < TableSize; i++)
            table[i] = new List<int>();

        hashMethod = method;
    }
    
    // Вставка ключа в хеш-таблицу
    public void Insert(int key)
    {
        int index = GetHash(key);
        if (index < 0 || index >= TableSize)
        {
            throw new IndexOutOfRangeException($"Хэш-функция вернула недопустимый индекс: {index}");
        }
        if (!table[index].Contains(key))
            table[index].Add(key);
    }

    // Поиск ключа в хеш-таблице
    public bool Search(int key)
    {
        int index = GetHash(key);
        return table[index].Contains(key);
    }

    // Удаление ключа из хеш-таблицы
    public bool Delete(int key)
    {
        int index = GetHash(key);
        return table[index].Remove(key);
    }
    
    // Выбор метода хеширования
    private int GetHash(int key)
    {
        switch (hashMethod)
        {
            case HashMethod.Division:
                return DivisionHash(key);
            case HashMethod.Multiplication:
                return MultiplicationHash(key);
            case HashMethod.XorConstantHash:
                return XorConstantHash(key);
            case HashMethod.RotateLeftHash:
                return RotateLeftHash(key);
            case HashMethod.XorAddHash:
                return XorAddHash(key);
            case HashMethod.PolynomialHash:
                return PolynomialHash(key);
            default:
                throw new InvalidOperationException("Неизвестный метод хэширования");
        }
    }
    
    // Метод деления
    private int DivisionHash(int key)
    {
        return key % TableSize;
    }

    // Метод умножения
    private int MultiplicationHash(int key)
    {
        double A = 0.6180339887; // Константа, 0 < A < 1
        return (int)(TableSize * (key * A % 1));
    }

    // Собственный метод хеширования (например, xor с константой)
    private int XorConstantHash(int key)
    {
        return (key ^ 1234567) % TableSize; // Простое преобразование с использованием XOR
    }
    
    // Метод с использованием циклического сдвига (Rotate Left)
    private int RotateLeftHash(int key)
    {
        return Math.Abs(((key << 5) | (key >> 27)) % TableSize);
    }
    
    // Метод с использованием XOR и сложения
    private int XorAddHash(int key)
    {
        return Math.Abs(((key ^ (key << 3)) + (key ^ (key >> 5))) % TableSize); // Добавлен % TableSize
    }
    
    // Метод с использованием полиномиального хэширования
    private int PolynomialHash(int key)
    {
        return Math.Abs((key * 31 + 17) % TableSize); // Добавлен % TableSize
    }
    
    // Статистика: длины цепочек
    public (int minLength, int maxLength) GetChainLengths()
    {
        int min = int.MaxValue, max = 0;
        foreach (var chain in table)
        {
            int length = chain.Count;
            if (length > max) max = length;
            if (length < min) min = length;
        }
        return (min, max);
    }

    // Подсчет коэффициента заполнения
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