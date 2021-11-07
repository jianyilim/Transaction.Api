namespace Transaction.Domain.Transactions
{
    public class TransactionXML
    {
        public string Id { get; set; }

        public TransactionXMLPaymentDetails PaymentDetails { get; set; }

        public TransactionStatus TransactionStatus { get; set; }
    }
}
