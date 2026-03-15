using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MotherTreeCrafts.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } 

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new InvalidOperationException("SendGridKey is not configured");
        }
        
        await Execute(Options.SendGridKey, subject, message, toEmail);
    }

    private async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            // Now pulling dynamically from settings!
            From = new EmailAddress(Options.SenderEmail, Options.SenderName ?? "Mother Tree Crafts"),
            
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        
        // This line automatically sends it to whoever just registered on the site
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking to prevent SendGrid from malforming the confirmation token links
        msg.SetClickTracking(false, false);
        
        var response = await client.SendEmailAsync(msg);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email to {ToEmail} queued successfully!", toEmail);
        }
        else
        {
            _logger.LogError("Failure sending email to {ToEmail}. Status Code: {StatusCode}", toEmail, response.StatusCode);
        }
    }
}