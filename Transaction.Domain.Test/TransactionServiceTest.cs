using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Transaction.Api.UnitTest.Helpers;
using Transaction.Domain.Transactions;
using Xunit;

namespace Transaction.Domain.Test
{
    public class TransactionServiceTest
    {
        [Theory]
        [MemberData(nameof(GetTransactionsTestCases))]
        public async Task GetTransactionsAsync(TestCase<TransactionSearchRequest, IEnumerable<TransactionResponse>> testCase)
        {
            ServiceFactory serviceFactory = new ServiceFactory();
            string exceptionMessage = null;
            IEnumerable<TransactionResponse> result = null;
            try
            {
                result = await serviceFactory.GetRequiredService<ITransactionService>().GetTransactionsAsync(testCase.Request);
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            Assert.Equal(JsonConvert.SerializeObject(testCase.ExpectedResult), JsonConvert.SerializeObject(result));
            Assert.Equal(testCase.ExceptionMessage, exceptionMessage);
        }

        public static IEnumerable<object[]> GetTransactionsTestCases
            => EmbeddedJsonResourceReaderHelper.GetMemberData<TestCase<TransactionSearchRequest, IEnumerable<TransactionResponse>>>
            ($"{typeof(TransactionServiceTest).Namespace}.{nameof(GetTransactionsTestCases)}.json");
    }

}
