using ParkingSpotApi.Interfaces;

namespace ParkingSpotApi;

public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();
    private int _id = 1;
    public IEnumerable<T> GetAll() => _items;
    public T? Find(int id) => _items.FirstOrDefault(x => x.Id == id);
    
    public void Add(T entity)
    {
        entity.Id = _id++;
        _items.Add(entity);
    }

    public bool Update(T entity)
    {
        var idx = _items.FindIndex(x => x.Id == entity.Id);
        if (idx >= 0)
        {
            _items[idx] = entity;
            return true;
        }
        return false;
    }

    public bool Remove(T entity)
    {
        return _items.Remove(entity);
    }
}