using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Transaction.Domain.Transactions
{
    public class TransactionImportRequest
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public FileFormat FileFormat { get; set; }
    }
}
