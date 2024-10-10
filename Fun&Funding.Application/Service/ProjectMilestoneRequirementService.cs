using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.IStorageService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ProjectMilestoneDTO;
using Fun_Funding.Application.ViewModel.ProjectMilestoneRequirementDTO;
using Fun_Funding.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Service
{
    public class ProjectMilestoneRequirementService : IProjectMilestoneRequirementService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IAzureService _azureService;
        public ProjectMilestoneRequirementService(IUnitOfWork unitOfWork, IMapper mapper, IAzureService azureService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _azureService = azureService;
        }
        public async Task<ResultDTO<string>> CreateMilestoneRequirements(List<ProjectMilestoneRequirementRequest> request)
        {
            try
            {
                foreach (var requestItem in request) {
                    ProjectMilestoneRequirement req = new ProjectMilestoneRequirement
                    {
                        ProjectMilestoneId = requestItem.ProjectMilestoneId,
                        Content = requestItem.Content,
                        RequirementId = requestItem.RequirementId
                    };
                    List<ProjectRequirementFile> files = new List<ProjectRequirementFile>();
                    if (requestItem.RequirementFiles?.Count > 0)
                    {
                        foreach (var file in requestItem.RequirementFiles)
                        {
                            if (file.URL.Length > 0)
                            {
                                var url = _azureService.UploadUrlSingleFiles(file.URL);
                                ProjectRequirementFile requirementFile = new ProjectRequirementFile
                                {
                                    Name = file.Name,
                                    URL = url.Result,
                                    CreatedDate = DateTime.Now,

                                };
                                files.Add(requirementFile);
                            }
                        }
                    }
                    req.RequirementFiles = files;
                    _unitOfWork.ProjectMilestoneRequirementRepository.Add(req);

                }
                 _unitOfWork.Commit();

                return ResultDTO<string>.Success("ok");
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public Task<ResultDTO<string>> UpdateMilestoneRequirements(List<ProjectMilestoneRequirementRequest> request)
        {
            throw new NotImplementedException();
        }
    }
}
