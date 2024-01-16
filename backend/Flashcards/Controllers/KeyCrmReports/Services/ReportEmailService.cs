using System.Net;
using System.Net.Mail;

namespace Flashcards.Controllers.KeyCrmReports.Services;

public class ReportEmailService
{
    private readonly IConfiguration _configuration;

    public ReportEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendReport(string recepientEmail, string fileName, Stream reportStream)
    {
        string smtpHost = "smtp.gmail.com";
        int smtpPort = 587;
        bool enableSsl = true;

        string smtpUsername = _configuration["Email:Username"];
        string smtpPassword = _configuration["Email:Password"];

        using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
        {
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = enableSsl;

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(smtpUsername);
            mailMessage.To.Add(recepientEmail);
            mailMessage.Subject = "Звіт шоурум ЇЇ";
            mailMessage.Body = "файл зі звітом в прикріплених";
            mailMessage.IsBodyHtml = false;

            mailMessage.Attachments.Add(new Attachment(reportStream, fileName, "text/csv"));

            smtpClient.Send(mailMessage);
        }
    }
}