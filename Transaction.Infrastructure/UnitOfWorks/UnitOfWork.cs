using System.Threading.Tasks;
using Member.Profile.Infrastructure.UnitOfWorks;
using Transaction.Domain.Interface;
using Transaction.Domain.Transactions;
using Transaction.Domain.UnitOfWorks;

namespace Transaction.Infrastructure.UnitOfWorks
{
    /// <inheritdoc/>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TransactionDbContext _dbContext;
        private IGenericRepository<Domain.Transactions.Transaction> _transactionRepository;

        /// <inheritdoc/>
        public UnitOfWork(TransactionDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <inheritdoc/>
        public IGenericRepository<Domain.Transactions.Transaction> TransactionRepository
        {
            get
            {
                if (this._transactionRepository == null)
                {
                    this._transactionRepository = new DbGenericRepository<Domain.Transactions.Transaction>(this._dbContext);
                }

                return this._transactionRepository;
            }
        }

        /// <inheritdoc/>
        public virtual Task<int> SaveAsync()
        {
            return this._dbContext.SaveChangesAsync();
        }
    }
}
