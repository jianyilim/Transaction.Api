using System.Collections.Generic;
using System.Linq;

namespace Transaction.Domain.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        void AttachAndDelete(T obj);
        void Delete(ICollection<T> objs);
        void Delete(T obj);
        IQueryable<T> Get();
        IQueryable<T> GetAsNoTracking();
        void Insert(ICollection<T> obj);
        T Insert(T obj);
        void Update(T obj, T value);
    }
}