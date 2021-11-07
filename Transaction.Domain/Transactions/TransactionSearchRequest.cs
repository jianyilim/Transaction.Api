using System;

namespace Transaction.Domain.Transactions
{
    public class TransactionSearchRequest
    {
        public string Currency { get; set; }
        
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public UnifiedTransactionStatus? Status { get; set; }
    }
}
