using FluentEmail.Core;
using Fun_Funding.Application.IEmailServices;
using Fun_Funding.Domain.Enum;
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

        public async Task SendEmailAsync(string toEmail, string subject, string content, EmailType type)
        {
            string body = string.Empty;
            switch (type)
            {
                case EmailType.ResetPassword:
                    body = GenerateResetPasswordEmailBody(content);
                    break;
                default:
                    body = GenerateResetPasswordEmailBody(content);
                    break;
            }
            await _fluentEmail
                .To(toEmail)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();
        }

        private string GenerateResetPasswordEmailBody(string content)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset Email</title>
    <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@400;600&display=swap' rel='stylesheet'>
    <style>
        body {{ 
            font-family: 'Poppins', sans-serif; 
            background-color: #EAEAEA; 
            color: #2F3645; 
            margin: 0; 
            padding: 20px 0;
        }}
        table {{
            width: 100%;
            height: 100%;
            background-color: #EAEAEA;
            text-align: center;
        }}
        .container {{
            max-width: 450px;
            background-color: #FFFFFF;
            border-radius: 10px;
            padding: 20px;
            border: 1px solid #FFFFFF;
            margin: 30px auto;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        h1 {{ 
            color: #2F3645; 
            font-size: 18px;
            margin-bottom: 15px;
        }}
        p {{ 
            line-height: 1.4;
            font-size: 14px;
            color: #2F3645;
            margin-left: 30px;
            margin-right: 30px;
        }}
        .btn {{ 
            display: inline-block; 
            padding: 10px 20px;
            font-size: 14px;
            background-color: #1BAA64; 
            color: #F5F7F8 !important; 
            text-decoration: none; 
            border-radius: 5px; 
            margin-top: 25px;
            width: 70%;
        }}
        footer {{ 
            margin-top: 30px;
            font-size: 12px;
            color: #777; 
        }}
    </style>
</head>
<body>
    <table>
        <tr>
            <td>
                <div class='container'>
                    <img src='https://i.ibb.co/SxKvYLH/Frame-155.png' alt='Fun&Funding Logo' width='200px' style='margin-bottom: 20px; margin-top: 10px' /> <!-- Smaller logo -->
                    <h1>Your password reset link is ready!</h1>
                    <p>The password is best not to use anything with Fun&Funding or you and your family, pick something unique!</p>
                    <a href='{content}' class='btn'>Reset Password</a>
                    <footer>
                        <p>Thanks,<br>The Fun&Funding Team</p>
                        <p>&copy; 2024 Fun&Funding</p>
                    </footer>
                </div>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
