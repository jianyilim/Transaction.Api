using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Exceptions;

namespace Transaction.Domain.Transactions
{
    [Table("Transaction")]
    [Index(nameof(CurrencyCode))]
    [Index(nameof(TransactionDate))]
    [Index(nameof(Status))]
    public class Transaction
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [StringLength(3)]
        public string CurrencyCode { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionStatus Status { get; set; }

        [Column(TypeName = "timestamp")]
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] Timestamp { get; set; }

        public void ValidateIdLength()
        {
            if (this.Id.Trim().Length > 50
                || this.Id.Trim().Length <= 0)
            {
                throw new TransactionException("Id length must be from 1 to 50.");
            }
        }

        public void ValidateCurrencyCodeLength()
        {
            if (this.CurrencyCode.Trim().Length != 3)
            {
                throw new TransactionException("CurrencyCode must be in ISO4217 format.");
            }
        }

        public void ValidateAmount()
        {
            if (this.Amount <= 0)
            {
                throw new TransactionException("Amount must be positive.");
            }
        }

        public void Validate()
        {
            this.ValidateIdLength();
            this.ValidateCurrencyCodeLength();
            this.ValidateAmount();
        }
    }
}
