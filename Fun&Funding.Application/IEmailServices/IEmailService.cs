using Fun_Funding.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.IEmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, EmailType type);
        Task SendReportAsync(string toEmail, string projectName, string userName, DateTime reportedDate, string reporter, string reason);
    }
}
