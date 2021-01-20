using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TroydonFitnessWebsite.Models.Products;
using TroydonFitnessWebsite.Models.UserAccounts;
using TroydonFitnessWebsite.Settings.Mail;

namespace TroydonFitnessWebsite.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(mailRequest.ToEmail));
            message.Subject = mailRequest.Subject;
            if (mailRequest.Attachments != null)
            {
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            Attachment att = new Attachment(new MemoryStream(fileBytes), file.FileName);
                            message.Attachments.Add(att);
                        }
                    }
                }
            }
            message.IsBodyHtml = false;
            message.Body = mailRequest.Body;
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(message);
        }

        public async Task SendWelcomeEmailAsync(WelcomeRequest request, string confirmAccountLink, string toEmail, string firstName)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Areas\\Identity\\Pages\\Account\\EmailTemplates\\RegistrationConfirmationEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            // below line replaces all the html content inside the WelcomeEmail.cshtml, using Replace.[whatever is inside this], isReplacedWithThis
            MailText = MailText.Replace("[firstname]", firstName).Replace("[email]", toEmail).Replace("[confirm account link]", confirmAccountLink);
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = $"Please confirm your email {firstName}";
            message.IsBodyHtml = true;
            message.Body = MailText;
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(message);
        }

        public async Task SendRoutineOrderConfirmationEmail(WelcomeRequest request, int numberOfItemsInOrder, string toEmail, string firstName,
            DateTime orderDate, Guid? orderNumber,
            List<string> productNames, List<decimal?> prices, List<int> quantities, List<PersonalTraining.SessionType?> sessionTypes, List<int?> lengthsOfRoutine, List<PersonalTraining.Difficulty?> experienceLevels)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Areas\\Identity\\Pages\\Account\\EmailTemplates\\OrderConfirmation.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();

            // make a string builder to hold all of the type of product ordered in a list to pass it all to the email
            // later in javascript we will split on commas to break them up and put into a list
            //StringBuilder productNameStringList = new StringBuilder();
            //StringBuilder pricesStringList = new StringBuilder();
            //StringBuilder quantityStringList = new StringBuilder();
            //StringBuilder sessionTypeStringList = new StringBuilder();
            //StringBuilder lengthsOfRoutinesStringList = new StringBuilder();
            //StringBuilder experienceLevelStringList = new StringBuilder();

            //productNames.ForEach(productName => productNameStringList.Append(productName + ","));
            //prices.ForEach(price => pricesStringList.Append(price + ","));
            //quantities.ForEach(quantity => quantityStringList.Append(quantity + ","));
            //sessionTypes.ForEach(sessionType => sessionTypeStringList.Append(sessionType + ","));
            //lengthsOfRoutine.ForEach(lengthOfRoutine => lengthsOfRoutinesStringList.Append(lengthOfRoutine + ","));
            //experienceLevels.ForEach(exp => experienceLevelStringList.Append(exp + ","));

            StringBuilder orderDetailsRowHtml = new StringBuilder();
            StringBuilder trainingOrderDetailsRowHtml = new StringBuilder();

            // if any of the elements is not null, then we can create the table
            if (sessionTypes.Any(i => i != null))
            {
                trainingOrderDetailsRowHtml.AppendLine("<table id='trainingOrder-detail-table' style='width:100%'>");
                trainingOrderDetailsRowHtml.AppendLine("<caption>Training Order Details</caption>");
                trainingOrderDetailsRowHtml.AppendLine("<tr>");
                trainingOrderDetailsRowHtml.AppendLine("<td>ProductName</td>");
                trainingOrderDetailsRowHtml.AppendLine("<td>Price</td>");
                trainingOrderDetailsRowHtml.AppendLine("<td>Quantity</td>");
                trainingOrderDetailsRowHtml.AppendLine("<td>Session Type</td>");
                trainingOrderDetailsRowHtml.AppendLine("<td>Length Of Routine</td>");
                trainingOrderDetailsRowHtml.AppendLine("<td>Experience Level</td>");
                trainingOrderDetailsRowHtml.AppendLine("</tr>");
            }

            for (int i = 0; i < numberOfItemsInOrder; i++) // + 10 because a comma is added each time { fsdds , fdsafdas}
            {
                // build the general order details table if no session type
                if (sessionTypes[i] == null)
                {
                    orderDetailsRowHtml.AppendLine("<tr>");
                    orderDetailsRowHtml.AppendLine($"<td>{productNames[i]}</td>");
                    orderDetailsRowHtml.AppendLine($"<td>{prices[i]}</td>");
                    orderDetailsRowHtml.AppendLine($"<td>{quantities[i]}</td>");
                    orderDetailsRowHtml.AppendLine("</tr>");
                }
                // otherwise build the
                else
                {
                    trainingOrderDetailsRowHtml.AppendLine("<tr>");
                    trainingOrderDetailsRowHtml.AppendLine($"<td>{productNames[i]}</td>");
                    trainingOrderDetailsRowHtml.AppendLine($"<td>{prices[i]}</td>");
                    trainingOrderDetailsRowHtml.AppendLine($"<td>{quantities[i]}</td>");
                    trainingOrderDetailsRowHtml.AppendLine($"<td>{sessionTypes[i]}</td>");
                    trainingOrderDetailsRowHtml.AppendLine($"<td>{lengthsOfRoutine[i]}</td>");
                    trainingOrderDetailsRowHtml.AppendLine($"<td>{experienceLevels[i]}</td>");
                    trainingOrderDetailsRowHtml.AppendLine("</tr>");
                }
            }

            // finish the table if there was any entry to the training routine table
            if (trainingOrderDetailsRowHtml.Length != 0) trainingOrderDetailsRowHtml.AppendLine("</table>");

            // below line replaces all the html content inside the WelcomeEmail.cshtml, using Replace.[whatever is inside this], isReplacedWithThis
            MailText = MailText.Replace("[firstname]", firstName).Replace("[email]", toEmail).Replace("[orderdate]", orderDate.ToString())
                    .Replace("[ordernumber]", orderNumber.ToString())
                    // the list objects to replace
                    .Replace("[orderdetails]",
                    $"<table id='order-detail-table' style='width:100%'>" +
                    $"<caption>Order Details</caption>" +
                    $"<tr>" +
                    $"     <td>ProductName</td>" +
                    $"     <td>Price</td>" +
                    $"     <td>Quantity</td>" +
                    $"</tr>" +
                    $" {orderDetailsRowHtml}" +
                    $"</table>"
                    )
                    .Replace("[trainingorderdetails]", trainingOrderDetailsRowHtml.ToString());

            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = $"TroydonFitness - Order Confirmation {firstName}";
            message.IsBodyHtml = true;
            message.Body = MailText;
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(message);
        }

        public async Task SendPasswordResetAsync(WelcomeRequest request, string resetPassLink, string toEmail, string firstName)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Areas\\Identity\\Pages\\Account\\EmailTemplates\\ResetPasswordEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            // below line replaces all the html content inside the WelcomeEmail.cshtml, using Replace.[whatever is inside this], isReplacedWithThis
            MailText = MailText.Replace("[firstname]", firstName).Replace("[email]", toEmail).Replace("[reset pass link]", resetPassLink);
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = $"Please confirm your email {firstName}";
            message.IsBodyHtml = true;
            message.Body = MailText;
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(message);
        }

        public async Task UpdateEmailAsync(WelcomeRequest request, string updateEmailLink, string toEmail, string firstName)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Areas\\Identity\\Pages\\Account\\EmailTemplates\\UpdateEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            // below line replaces all the html content inside the WelcomeEmail.cshtml, using Replace.[whatever is inside this], isReplacedWithThis
            MailText = MailText.Replace("[firstname]", firstName).Replace("[email]", toEmail).Replace("[update email link]", updateEmailLink);
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = $"Please confirm your new email {firstName}";
            message.IsBodyHtml = true;
            message.Body = MailText;
            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(message);
        }
    }
}