using FluentEmail.Core;
using Fun_Funding.Application.IEmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Infrastructure.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            await _fluentEmail
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .SendAsync();
        }
    }
}
