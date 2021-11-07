using System;
using System.ComponentModel.DataAnnotations;

namespace Transaction.Domain.Transactions
{
    public class TransactionSearchRequest
    {
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must in ISO4217 format.")]
        public string CurrencyCode { get; set; }
        
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public UnifiedTransactionStatus? Status { get; set; }
    }
}
