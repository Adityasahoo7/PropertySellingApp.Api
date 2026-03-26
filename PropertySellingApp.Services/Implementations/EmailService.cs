using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PropertySellingApp.Models.Entities;
using PropertySellingApp.Services.Interfaces;
using System.Security.Authentication;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> smtpOptions)
    {
        _smtp = smtpOptions.Value;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            "Property Selling App",
            _smtp.FromEmail
        ));

        message.Sender = MailboxAddress.Parse(_smtp.FromEmail);
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Login Verification Code";

        message.Body = new TextPart("html")
        {
            Text = $@"
        <html>
          <body style='font-family: Arial;'>
            <h3>Login Verification</h3>
            <p>Your OTP code:</p>
            <h2>{otp}</h2>
            <p>This code is valid for 5 minutes.</p>
            <p>Please do not share this code.</p>
            <br/>
            <p>Regards,<br/>Property Selling App</p>
          </body>
        </html>"
        };

        using var client = new SmtpClient();
        client.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
        client.LocalDomain = "apmosys.com";

        await client.ConnectAsync(
            _smtp.Host,
            587,
            SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
