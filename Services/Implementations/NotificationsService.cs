using Microsoft.AspNetCore.Mvc;
using MimeKit;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services.Implementations
{
    public class NotificationsService : INotificationsService
    {
        private readonly ILogger<NotificationsService> _logger;
        private readonly ApplicationDatas _appData;

        public NotificationsService(ILogger<NotificationsService> logger,
            ApplicationDatas appData)
        {
            _logger = logger;
            _appData = appData;
        }

        public async Task<IActionResult> SendWeatherAlertAsync(string email, string city, string message)
        {
            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

            if (Validators.IsEmailValid(email))
            {
                try
                {
                    smtpClient.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.Auto);
                    smtpClient.Authenticate(_appData.FirstMail, _appData.Password);

                    var localMessage = new MimeMessage();

                    localMessage.From.Add(new MailboxAddress("WeatherApp", _appData.FirstMail));
                    localMessage.To.Add(new MailboxAddress("You", email));

                    localMessage.Subject = "Weather Alert";

                    var part = new TextPart("plain")
                    {
                        Text = $"Weather alert for the {city} city: {message}."
                    };

                    localMessage.Body = part;
                    smtpClient.Send(localMessage);
                }
                catch (Exception ex) when (ex is InvalidOperationException or ArgumentNullException or InvalidCastException)
                {
                    _logger.LogError(ex, "An error occurred in the SendFeedbackAsync method.");
                    return new StatusCodeResult(400);
                }
                finally
                {
                    smtpClient.Disconnect(true);
                }

                return new StatusCodeResult(200);
            }

            _logger.LogError("Mail is not valid.");
            return new StatusCodeResult(400);
        }
    }
}
