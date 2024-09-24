using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Infrastructure.Database;
using Fun_Funding.Application.ViewModel.PackageBackerDTO;
using Fun_Funding.Application.IService;
using Azure.Core;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageBackersController : ControllerBase
    {
        private readonly IPackageBackerService _packageBackerService;

        public PackageBackersController(IPackageBackerService packageBackerService)
        {
            _packageBackerService = packageBackerService;
        }

        [HttpPost]
        public async Task<IActionResult> DonateFundingProject([FromBody] PackageBackerRequest packageBackerRequest)
        {
            var result = await _packageBackerService.DonateFundingProject(packageBackerRequest);

            if (result._isSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
