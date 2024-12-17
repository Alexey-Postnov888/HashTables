namespace HashTables;

public class HashTableOpenAddressing : IHashTable
{
    private static int TableSize = 10000;
    private int?[] table; // Массив для хранения ключей
    private int currentSize; // Текущий размер таблицы

    public enum CollisionResolution
    {
        LinearProbing,
        QuadraticProbing,
        DoubleHashing,
        ModuloOffsetProbing,
        XorConstantProbing
    }

    private CollisionResolution resolutionMethod;

    public HashTableOpenAddressing(CollisionResolution method)
    {
        table = new int?[TableSize];
        currentSize = 0;
        resolutionMethod = method;
    }
    
    // Вставка ключа в хеш-таблицу
    public void Insert(int key)
    {
        if ((double)currentSize / TableSize >= 0.5) // Если таблица заполнена > 50%, увеличиваем размер
        {
            Rehash();
        }

        int index = GetHash(key);
        int i = 0;

        while (i < TableSize) // Ограничение попыток поиска свободного места
        {
            int probeIndex = GetProbeIndex(index, key, i);
            if (table[probeIndex] == null)
            {
                table[probeIndex] = key;
                currentSize++;
                return;
            }
            i++;
        }

        throw new InvalidOperationException("Failed to insert key: Table is full or probing failed.");
    }

    private void Rehash()
    {
        Console.WriteLine("Rehashing table...");
        int newSize = GetNextPrime(TableSize * 2); // Следующее простое число
        int?[] oldTable = table;

        table = new int?[newSize];
        TableSize = newSize; // Обновляем размер таблицы
        currentSize = 0;

        foreach (var key in oldTable)
        {
            if (key.HasValue)
            {
                Insert(key.Value); // Корректно вставляем элементы в новую таблицу
            }
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




    // Поиск ключа в хеш-таблице
    public bool Search(int key)
    {
        int index = GetHash(key);
        int i = 0;

        while (table[GetProbeIndex(index, key, i)] != null)
        {
            if (table[GetProbeIndex(index, key, i)] == key)
                return true;
            i++;
        }
        return false;
    }
    
    // Удаление ключа из хеш-таблицы
    public bool Delete(int key)
    {
        int index = GetHash(key);
        int i = 0;

        while (table[GetProbeIndex(index, key, i)] != null)
        {
            if (table[GetProbeIndex(index, key, i)] == key)
            {
                table[GetProbeIndex(index, key, i)] = null;
                currentSize--;
                return true;
            }
            i++;
        }
        return false;
    }

    private int GetHash(int key)
    {
        return key % TableSize;
    }
    
    private int GetProbeIndex(int hash, int key, int i)
    {
        switch (resolutionMethod)
        {
            case CollisionResolution.LinearProbing:
                return (hash + i) % TableSize;
            case CollisionResolution.QuadraticProbing:
                int quadraticIndex = (hash + i * i) % TableSize;
                if (i >= TableSize) throw new InvalidOperationException("Quadratic probing failed: Table full or infinite loop detected.");
                return quadraticIndex;
            case CollisionResolution.DoubleHashing:
                return (hash + i * SecondHash(key)) % TableSize;
            case CollisionResolution.ModuloOffsetProbing:
                return (hash + i * (key % 7 + 1)) % TableSize;
            case CollisionResolution.XorConstantProbing:
                return (hash + i * ((key ^ 12345) % TableSize)) % TableSize;
            default:
                throw new InvalidOperationException("Unknown collision resolution method");
        }
    }

    
    private int SecondHash(int key)
    {
        return 7 - (key % 7);
    }

    // Подсчет длины самого длинного кластера
    public int GetLongestCluster()
    {
        int maxCluster = 0, currentCluster = 0;
        foreach (var cell in table)
        {
            if (cell != null)
                currentCluster++;
            else
            {
                if (currentCluster > maxCluster) maxCluster = currentCluster;
                currentCluster = 0;
            }
        }
        return Math.Max(maxCluster, currentCluster);
    }
}