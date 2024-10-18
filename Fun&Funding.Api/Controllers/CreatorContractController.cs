using Fun_Funding.Application;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.CreatorContractDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatorContractController : ControllerBase
    {
        private readonly ICreatorContractService _contractService;
        public CreatorContractController(ICreatorContractService contractService) {
           _contractService = contractService;
        }
        [HttpPost]
        public IActionResult CreateContract([FromBody] CreatorContractRequest contract)
        {
            var result = _contractService.CreateContract(contract);
            return Ok(result);
        }
    }
}
