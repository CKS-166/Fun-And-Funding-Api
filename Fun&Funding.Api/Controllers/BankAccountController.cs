using Fun_Funding.Application.Interfaces.IEntityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/bank-accounts")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankAccountByWalletId(Guid id)
        {
            var result = await _bankAccountService.GetBankAccountByWalletId(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
