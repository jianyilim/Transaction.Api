using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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


        public static void Configure(ModelBuilder modelBuilder)
        {

        }
    }
}
