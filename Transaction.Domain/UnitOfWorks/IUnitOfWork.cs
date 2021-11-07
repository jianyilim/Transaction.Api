using System.Threading.Tasks;
using Transaction.Domain.Interface;

namespace Transaction.Domain.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IGenericRepository<Transactions.Transaction> TransactionRepository { get; }

        Task<int> SaveAsync();
    }
}
