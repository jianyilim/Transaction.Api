using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
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

        public async Task ImportTransactionsAsync(Stream stream, FileFormat fileFormat)
        {
            IEnumerable<Transaction> transactions = fileFormat == FileFormat.CSV
                ? this.ReadCSV(stream)
                : this.ReadXML(stream);

            foreach (Transaction transaction in transactions)
            {
                transaction.Validate();

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

        private IEnumerable<Transaction> ReadXML(Stream stream)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TransactionsXML));
                serializer.UnknownNode += new
                    XmlNodeEventHandler(this.serializer_UnknownNode);
                serializer.UnknownAttribute += new
                    XmlAttributeEventHandler(this.serializer_UnknownAttribute);

                using TextReader reader = new StreamReader(stream);
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
            catch (InvalidOperationException invalidOperationException)
            {
                this._logger.LogError(invalidOperationException, invalidOperationException.Message);
                throw new Exception("Unknown format");
            }
        }

        private IEnumerable<Transaction> ReadCSV(Stream stream)
        {
            try
            {
                CsvConfiguration config = new CsvConfiguration(new CultureInfo("en-GB"))
                {
                    HasHeaderRecord = false,
                };
                using TextReader reader = new StreamReader(stream);
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
            catch (CsvHelperException csvHelperException)
            {
                this._logger.LogError(csvHelperException, csvHelperException.Message);
                throw new Exception("Unknown format");
            }
        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            this._logger.LogError("UnknownNode={UnknownNode} Text={Text}", e.Name, e.Text);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            this._logger.LogError("UnknownAttribute={UnknownAttribute} Value={Value}", attr.Name, attr.Value);
        }
    }
}
