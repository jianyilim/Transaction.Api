using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Transaction.Domain.UnitOfWorks;

namespace Transaction.Domain.Transactions
{
    public class TransactionImportService : ITransactionImportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionImportService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task ImportTransactionsAsync(TransactionImportRequest transactionImportRequest)
        {
            IEnumerable<Transaction> transactions = transactionImportRequest.FileFormat == FileFormat.CSV
                ? this.ReadCSV(transactionImportRequest.File)
                : this.ReadXML(transactionImportRequest.File);

            foreach (Transaction transaction in transactions)
            {
                Transaction transactionInDb = await this._unitOfWork.TransactionRepository
                      .Get()
                      .Where(t => t.Id == transaction.Id)
                      .FirstOrDefaultAsync();

                if (transactionInDb is null)
                {
                    this._unitOfWork.TransactionRepository.Insert(transaction);
                }
                else
                {
                    this._unitOfWork.TransactionRepository.Update(transactionInDb, transaction);
                }
            }
            await this._unitOfWork.SaveAsync();
        }

        private IEnumerable<Transaction> ReadXML(IFormFile formFile)
        {
            CsvConfiguration config = new CsvConfiguration(new CultureInfo("en-GB"))
            {
                HasHeaderRecord = false,
            };
            using TextReader reader = new StreamReader(formFile.OpenReadStream());
            using CsvReader csvReader = new CsvReader(reader, config);
            csvReader.Context.RegisterClassMap<TransactionCSVMap>();
            IEnumerable<TransactionCSV> value = csvReader.GetRecords<TransactionCSV>();
            return value
                .Select(transactionCSV => new Transaction
                {
                    Id = transactionCSV.Id.Trim(),
                    Amount = transactionCSV.Amount,
                    CurrencyCode = transactionCSV.CurrencyCode.Trim(),
                    TransactionDate = transactionCSV.TransactionDate,
                    Status = (TransactionStatus)(int)transactionCSV.Status,
                });
        }

        private IEnumerable<Transaction> ReadCSV(IFormFile formFile)
        {
            CsvConfiguration config = new CsvConfiguration(new CultureInfo("en-GB"))
            {
                HasHeaderRecord = false,
            };
            using TextReader reader = new StreamReader(formFile.OpenReadStream());
            using CsvReader csvReader = new CsvReader(reader, config);
            csvReader.Context.RegisterClassMap<TransactionCSVMap>();
            IEnumerable<TransactionCSV> value = csvReader.GetRecords<TransactionCSV>();
            return value
                .Select(transactionCSV => new Transaction
                { 
                    Id = transactionCSV.Id.Trim(),
                    Amount = transactionCSV.Amount,
                    CurrencyCode = transactionCSV.CurrencyCode.Trim(),
                    TransactionDate = transactionCSV.TransactionDate,
                    Status = (TransactionStatus)(int)transactionCSV.Status,
                })
                .ToList();
        }
    }
}
