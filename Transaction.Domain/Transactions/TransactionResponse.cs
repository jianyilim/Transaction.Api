using System.Text.Json.Serialization;

namespace Transaction.Domain.Transactions
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public string Payment { get; set; }
        public UnifiedTransactionStatus Status { get; set; }
    }
}
