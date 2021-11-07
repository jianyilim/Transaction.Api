using System.Threading.Tasks;

namespace Transaction.Domain.Transactions
{
    public interface ITransactionImportService
    {
        Task ImportTransactionsAsync(TransactionImportRequest transactionImportRequest);
    }
}