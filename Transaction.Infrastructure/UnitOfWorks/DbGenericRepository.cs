using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Interface;

namespace Member.Profile.Infrastructure.UnitOfWorks
{
    /// <inheritdoc/>
    public class DbGenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _table;
        private readonly DbContext _dbContext;

        /// <inheritdoc/>
        public DbGenericRepository(DbContext dbContext)
        {
            this._dbContext = dbContext;
            this._table = dbContext.Set<T>();
        }

        /// <inheritdoc/>
        public virtual IQueryable<T> Get()
        {
            return this._table.AsQueryable();
        }

        /// <inheritdoc/>

        public virtual IQueryable<T> GetAsNoTracking()
        {
            return this._table.AsQueryable()
                .AsNoTracking();
        }

        /// <inheritdoc/>
        public virtual T Insert(T obj)
        {
            this._table.Add(obj);

            return obj;
        }

        /// <inheritdoc/>
        public virtual void Insert(ICollection<T> obj)
        {
            this._table.AddRange(obj);
        }

        /// <inheritdoc/>
        public virtual void Delete(T obj)
        {
            this._table.Remove(obj);
        }

        /// <inheritdoc/>
        public virtual void Delete(ICollection<T> objs)
        {
            this._table.RemoveRange(objs);
        }

        /// <inheritdoc/>
        public virtual void AttachAndDelete(T obj)
        {
            this._table.Attach(obj);
            this._table.Remove(obj);
        }

        public virtual void Update(T obj, T value)
        {
            this._dbContext.Entry(obj).CurrentValues.SetValues(value);
        }
    }
}
