using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.ReportDTO;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IService
{
    public interface IReportService
    {
        Task<ResultDTO<ViolientReport>> CreateReportRequest(ReportRequest request);
        Task<ResultDTO<PaginatedResponse<ViolientReport>>> GetAllReport(ListRequest request);
        Task<ResultDTO<ViolientReport>> UpdateHandleReport(HandleRequest request);

    }
}
