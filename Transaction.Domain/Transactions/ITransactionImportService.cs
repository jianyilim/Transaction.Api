using System.IO;
using System.Threading.Tasks;

namespace Transaction.Domain.Transactions
{
    public interface ITransactionImportService
    {
        Task ImportTransactionsAsync(Stream stream, FileFormat fileFormat);
    }
}