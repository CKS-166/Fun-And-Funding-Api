using Fun_Funding.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService) {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransByProjectId(Guid projectId) {
            var res = await _transactionService.GetAllTransactionsByProjectId(projectId);
            return Ok(res);
        }
    }
}
