namespace HashTables
{
    public interface IHashTable<TKey, TValue>
    {
        void Insert(TKey key, TValue value); // Вставка или обновление пары ключ-значение
        bool TryGetValue(TKey key, out TValue value); // Поиск значения по ключу
        bool Remove(TKey key); // Удаление пары по ключу
    }
}