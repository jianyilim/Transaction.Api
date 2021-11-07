using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transaction.Domain.Transactions
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponse>> GetTransactionsAsync(TransactionSearchRequest transactionSearchRequest);
    }
}