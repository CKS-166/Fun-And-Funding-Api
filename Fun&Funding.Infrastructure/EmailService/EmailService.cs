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

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            string body = GenerateEmailBody(content);

            await _fluentEmail
                .To(toEmail)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();
        }

        private string GenerateEmailBody(string content)
        {
            return $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Welcome Email</title>
        <style>
            body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; margin: 0; padding: 20px; }}
            .container {{ max-width: 600px; margin: auto; background: #fff; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); padding: 20px; }}
            h1 {{ color: #4CAF50; }}
            p {{ line-height: 1.6; }}
            footer {{ margin-top: 20px; font-size: 0.9em; color: #777; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <a href=""https://imgbb.com/""><img src=""https://i.ibb.co/MCyXDyg/Group-44.png"" alt=""Group-44"" border=""0""></a>
            <h1>Welcome, gamers!</h1>
            <p>{content}</p>
            <p>Thank you for joining us! We’re thrilled to have you on board. Stay tuned for updates and enjoy your experience with us!</p>
            <footer>
                <p>&copy; 2024 Fun&Funding</p>
            </footer>
        </div>
    </body>
    </html>";
        }

    }
}
