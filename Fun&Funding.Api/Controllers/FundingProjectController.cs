using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fun_Funding.Application.IStorageService;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
namespace Fun_Funding.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundingProjectController : ControllerBase
    {
        private IAzureService _storageService;
        private IFundingProjectService _fundingProjectService;
        public FundingProjectController(IAzureService storageService
            , IFundingProjectService fundingProjectService)
        {
            _storageService = storageService;
            _fundingProjectService = fundingProjectService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            var response = await _storageService.UploadFiles(files);
            return Ok(response);
        }

        [HttpPost("blob")]
        public async Task<IActionResult> UploadBlobFiles(List<IFormFile> files)
        {
            var response = await _storageService.UploadBlobFiles(files);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUploadFiles()
        {
            var response = await _storageService.GetUploadedItems();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] FundingProjectAddRequest req)
        {
            var response = await _fundingProjectService.CreateFundingProject(req);
            return Ok(response);
        }
    }


}
