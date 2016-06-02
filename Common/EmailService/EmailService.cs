using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace Common.EmailService
{

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            // Credentials:
            var credentialUserName = "joracristi@gmail.com";
            var sentFrom = "joracristi@gmail.com";
            var pwd = "jhctrbbwprqcaizq";

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(credentialUserName, pwd),
                    EnableSsl = true
                };


            // Creatte the credentials:
            NetworkCredential credentials = new NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = true;
            client.Credentials = credentials;

            // Create the message:
            var mail =
                new System.Net.Mail.MailMessage(sentFrom, message.Destination)
                {
                    Subject = message.Subject,
                    Body = message.Body
                };


            // Send:
           await client.SendMailAsync(mail);
        }
    }
}
