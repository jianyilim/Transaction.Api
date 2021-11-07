using System;
using System.ComponentModel.DataAnnotations;

namespace Transaction.Domain.Transactions
{
    public class TransactionCSV
    {
        [StringLength(50)]
        public string Id { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [StringLength(3)]
        public string CurrencyCode { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionCSVStatus Status { get; set; }
    }
}
