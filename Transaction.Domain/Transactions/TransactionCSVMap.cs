using CsvHelper.Configuration;

namespace Transaction.Domain.Transactions
{
    public sealed class TransactionCSVMap : ClassMap<Transaction>
    {
        public TransactionCSVMap()
        {
            this.Map(m => m.Id).Index(0);
            this.Map(m => m.Amount).Index(1);
            this.Map(m => m.CurrencyCode).Index(2);
            this.Map(m => m.TransactionDate).Index(3);
            this.Map(m => m.Status).Index(4);
        }
    }
}
