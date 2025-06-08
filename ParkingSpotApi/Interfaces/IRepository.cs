namespace ParkingSpotApi.Interfaces;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? Find(int id);
    void Add(T entity);
    bool Update(T entity);
    bool Remove(T entity);
}