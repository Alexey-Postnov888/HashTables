namespace HashTables;

public interface IHashTable
{
    void Insert(int key);
    bool Search(int key);
    bool Delete(int key);
}