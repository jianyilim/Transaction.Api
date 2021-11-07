using Transaction.Domain.Transactions;

namespace Transaction.Domain.Test
{
    public class TransactionImportServiceTestRequest
    {
        public FileFormat FileFormat { get; set; }

        public string FileName { get; set; }
    }

}
