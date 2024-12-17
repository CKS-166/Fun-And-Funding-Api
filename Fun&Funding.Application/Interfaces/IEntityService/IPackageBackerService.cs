﻿using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.PackageBackerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IPackageBackerService
    {
        Task<ResultDTO<PackageBackerResponse>> DonateFundingProject(PackageBackerRequest packageBackerRequest);
        Task<ResultDTO<List<DonationResponse>>> ViewDonationById(Guid id);
        Task<ResultDTO<List<PackageBackerGroupedResponse>>> GetGroupedPackageBackersAsync(Guid projectId);

        Task<ResultDTO<List<PackageBackerCountResponse>>> GetPackageBackerGroups(Guid projectId);

        Task<ResultDTO<IEnumerable<object>>> GetProjectBacker(Guid projectId);
    }
}
