using System.Xml.Serialization;

namespace Transaction.Domain.Transactions
{
    [XmlRoot("Transactions", IsNullable = false)]
    public class TransactionsXML
    {
        [XmlElement("Transaction")]
        public TransactionXML[] Transactions { get; set; }
    }
}
