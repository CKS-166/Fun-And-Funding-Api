using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IFundingProjectService
    {
        Task<ResultDTO<FundingProjectResponse>> GetProjectById(Guid id);
        Task<ResultDTO<FundingProjectResponse>> CreateFundingProject(FundingProjectAddRequest req);

        Task<ResultDTO<string>> UpdateFundingProject(FundingProjectUpdateRequest req);


    }
}
