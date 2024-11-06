using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Application.Services.ExternalServices;
using Fun_Funding.Application.ViewModel.PackageBackerDTO;
using Fun_Funding.Application.IService;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/package-backers")]
    [ApiController]
    public class PackageBackersController : ControllerBase
    {
        private readonly IPackageBackerService _packageBackerService;

        public PackageBackersController(IPackageBackerService packageBackerService)
        {
            _packageBackerService = packageBackerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetDonationById(Guid userId) { 
            var result = await _packageBackerService.ViewDonationById(userId);
            if (result._isSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost]
        [Authorize (Roles = "Backer, Owner")]
        public async Task<IActionResult> DonateFundingProject([FromBody] PackageBackerRequest packageBackerRequest)
        {
            var result = await _packageBackerService.DonateFundingProject(packageBackerRequest);

            if (result._isSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("project-backers")]
        public async Task<IActionResult> GetPackageBackersByProject([FromQuery] Guid projectId)
        {
            var result = _packageBackerService.GetGroupedPackageBackersAsync(projectId);
            return Ok(result);
        }
    }
}
