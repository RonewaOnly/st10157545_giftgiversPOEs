using MailKit.Net.Smtp;
using MimeKit;
using System.Net;
using System.Net.Mail;
namespace st10157545_giftgiversPOEs.Services
{
    public class EmailService
    {
        //private readonly SmtpSettings _smtpSettings;

        //public EmailService(IOptions<SmtpSettings> smtpSettings)
        //{
        //    _smtpSettings = smtpSettings.Value;

        //    _smtpSettings = smtpSettings?.Value ?? new SmtpSettings
        //    {
        //        Server = "smtp-relay.brevo.com",
        //        Port = 587,
        //        Username = "7ca457001@smtp-brevo.com",
        //        Password = "RN0bsJtkwx78HT1X",
        //        SenderName = "student",
        //        SenderEmail = "www.rsmaselesele200@gmail.com"
        //    };

        //    // Debugging output
        //    Console.WriteLine($"SMTP Server: {_smtpSettings.Server}");
        //    Console.WriteLine($"SMTP Port: {_smtpSettings.Port}");
        //    Console.WriteLine($"Username: {_smtpSettings.Username}");

        //}

        //public async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        //    message.To.Add(new MailboxAddress("Volunteer's", toEmail));
        //    message.Subject = subject;

        //    var bodyBuilder = new BodyBuilder
        //    {
        //        HtmlBody = body
        //    };
        //    message.Body = bodyBuilder.ToMessageBody();

        //    using (var client = new SmtpClient())
        //    {
        //        try
        //        {
        //            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
        //            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
        //            await client.SendAsync(message);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Exception: {ex.Message}");
        //            throw;
        //        }
        //        finally
        //        {
        //            await client.DisconnectAsync(true);
        //        }
        //    }
        //}
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = message
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    } 

}
