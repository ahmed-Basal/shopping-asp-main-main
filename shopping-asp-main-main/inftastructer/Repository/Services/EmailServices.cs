using core.Dto;
using core.Services;
using core.shareing;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
//using MailKit.Net.Smtp;
//using MailKit.Security;

namespace inftastructer.Repository.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(EmailDto emailDto)
        {
            if (emailDto == null)
                throw new ArgumentNullException(nameof(emailDto));
           
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_configuration["EmailSetting:form"]));
            message.To.Add(MailboxAddress.Parse(emailDto.to));
            message.Subject = emailDto.subject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDto.content
            };
 //var emailBody = Emailstringbody.sendemail(message, code, userId);
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                int port = int.TryParse(_configuration["EmailSetting:port"], out var p) ? p : 587;

                await smtp.ConnectAsync(
                    _configuration["EmailSetting:host"],
                    port,
                    MailKit.Security.SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["EmailSetting:form"],
                    _configuration["EmailSetting:password"]);

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email", ex);
            }
        }
    }
}
