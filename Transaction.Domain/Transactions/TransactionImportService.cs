using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Transaction.Domain.UnitOfWorks;

namespace Transaction.Domain.Transactions
{
    public class TransactionImportService : ITransactionImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TransactionImportService(IUnitOfWork unitOfWork, ILogger<TransactionImportService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
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
            XmlSerializer serializer = new XmlSerializer(typeof(TransactionsXML));
            serializer.UnknownNode += new
                XmlNodeEventHandler(this.serializer_UnknownNode);
            serializer.UnknownAttribute += new
                XmlAttributeEventHandler(this.serializer_UnknownAttribute);

            using TextReader reader = new StreamReader(formFile.OpenReadStream());
            TransactionsXML transactions = (TransactionsXML)serializer.Deserialize(reader);
            return transactions.Transactions
                .Select(transactionXML => new Transaction
                {
                    Id = transactionXML.Id.Trim(),
                    Amount = transactionXML.PaymentDetails.Amount,
                    CurrencyCode = transactionXML.PaymentDetails.CurrencyCode.Trim(),
                    TransactionDate = transactionXML.TransactionDate,
                    Status = transactionXML.Status,
                })
                .ToList();
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
        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            this._logger.LogError("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            this._logger.LogError("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }
    }
}
