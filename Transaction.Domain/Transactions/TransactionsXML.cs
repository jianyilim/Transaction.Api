using System.Xml.Serialization;

namespace Transaction.Domain.Transactions
{
    [XmlRoot("Transactions", IsNullable = false)]
    public class TransactionsXML
    {
        [XmlArray("Items")]
        public TransactionXML[] Transactions { get; set; }
    }
}
