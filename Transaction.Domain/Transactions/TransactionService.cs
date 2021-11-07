using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Transaction.Domain.UnitOfWorks;

namespace Transaction.Domain.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TransactionResponse>> GetTransactionsAsync(TransactionSearchRequest transactionSearchRequest)
        {
            IQueryable<Transaction> query = this._unitOfWork.TransactionRepository.GetAsNoTracking();

            if (!string.IsNullOrEmpty(transactionSearchRequest.Currency))
            {
                query = query
                    .Where(transaction => transaction.CurrencyCode == transactionSearchRequest.Currency);
            }

            if (transactionSearchRequest.FromDate != null)
            {
                query = query
                    .Where(transaction => transaction.TransactionDate >= transactionSearchRequest.FromDate);
            }

            if (transactionSearchRequest.ToDate != null)
            {
                query = query
                    .Where(transaction => transaction.TransactionDate <= transactionSearchRequest.ToDate);
            }

            if (transactionSearchRequest.Status != null)
            {
                query = query
                    .Where(transaction => transaction.Status == (TransactionStatus)(int)transactionSearchRequest.Status);
            }

            return (await query
                .ToListAsync())
                .Select(transaction => new TransactionResponse
                { 
                    Id = transaction.Id,
                    Payment =$"{transaction.Amount:0.00} {transaction.CurrencyCode}",
                    Status = (UnifiedTransactionStatus)(int)transaction.Status
                });
        }
    }
}
