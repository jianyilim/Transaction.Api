using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transaction.Domain.Transactions;

namespace Transaction.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class TransactionController : ControllerBase
    {
        /// <summary>
        /// Import transactions.
        /// </summary>
        /// <param name="transactionImportRequest">File.</param>
        /// <param name="transactionImportService">Import service object.</param>
        /// <returns>Status code OK if succeeded.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [RequestFormLimits(MultipartBodyLengthLimit = 1048576)]
        public Task ImportTransactionsAsync([FromForm] TransactionImportRequest transactionImportRequest, [FromServices] ITransactionImportService transactionImportService)
        {
            return transactionImportService.ImportTransactionsAsync(transactionImportRequest);
        }

        /// <summary>
        /// Get transactions.
        /// </summary>
        /// <returns>A list of transactions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
        public Task<IEnumerable<TransactionResponse>> GetAsync([FromServices] ITransactionService transactionService, [FromQuery] TransactionSearchRequest transactionSearchRequest)
        {
            return transactionService.GetTransactionsAsync(transactionSearchRequest);
        }
    }
}
