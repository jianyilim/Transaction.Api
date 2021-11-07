using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Transaction.Api.UnitTest.Helpers;
using Transaction.Domain.Transactions;
using Transaction.Domain.UnitOfWorks;
using Xunit;

namespace Transaction.Domain.Test
{
    public class TransactionImportServiceTest
    {
        [Theory]
        [MemberData(nameof(ImportTransactionsTestCases))]
        public async Task ImportTransactionsAsync(TestCase<TransactionImportServiceTestRequest, IEnumerable<Transactions.Transaction>> testCase)
        {
            ServiceFactory serviceFactory = new ServiceFactory(false, nameof(TransactionImportServiceTest));
            string exceptionMessage = null;
            try
            {
                using System.IO.Stream stream = EmbeddedJsonResourceReaderHelper.GetFileStream(testCase.Request.FileName);
                await serviceFactory.GetRequiredService<ITransactionImportService>().ImportTransactionsAsync(stream, testCase.Request.FileFormat);
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }
            IUnitOfWork unitOfWork = serviceFactory.GetRequiredService<IUnitOfWork>();
            Assert.Equal(JsonConvert.SerializeObject(testCase.ExpectedResult), JsonConvert.SerializeObject(unitOfWork.TransactionRepository.GetAsNoTracking()));
            Assert.Equal(testCase.ExceptionMessage, exceptionMessage);
        }

        public static IEnumerable<object[]> ImportTransactionsTestCases
            => EmbeddedJsonResourceReaderHelper.GetMemberData<TestCase<TransactionImportServiceTestRequest, IEnumerable<Transactions.Transaction>>>
            ($"{typeof(TransactionImportServiceTest).Namespace}.{nameof(ImportTransactionsTestCases)}.json");
    }

}
