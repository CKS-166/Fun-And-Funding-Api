using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class FundingProjectManagementService : IFundingProjectService
    {
        public IUnitOfWork _unitOfWork;
        public IMapper _mapper;
        public FundingProjectManagementService(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ResultDTO<string>> CreateFundingProject(FundingProjectAddRequest projectRequest)
        {
            try
            {
                FundingProject project = _mapper.Map<FundingProject>(projectRequest);
                return ResultDTO<string>.Success("Add Sucessfully");
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public Task<ResultDTO> GetProject(int id)
        {
            throw new NotImplementedException();
        }
    }
}
