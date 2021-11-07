using System;
using System.Xml.Serialization;

namespace Transaction.Domain.Transactions
{
    
    public class TransactionXML
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionXMLPaymentDetails PaymentDetails { get; set; }

        public TransactionStatus? Status { get; set; }

    }
}
