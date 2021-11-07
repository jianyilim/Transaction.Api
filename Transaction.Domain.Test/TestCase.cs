namespace Transaction.Domain.Test
{
    public class TestCase<TRequest, TResult>
    {
        public TRequest Request { get; set; }

        public TResult ExpectedResult { get; set; }

        public string ExceptionMessage { get; set; }
    }
}
