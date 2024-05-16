using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Services.Interfaces;
using BCrypt.Net;
using BC = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace WeatherApp.Services.Implementations
{
    public class AccountService: IAccountService
    {
        private readonly WeatherAppDbContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(WeatherAppDbContext context, ILogger<AccountService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> ChangePasswordAsync(string login, string oldPassword, string newPassword)
        {
            try
            {
                var password = await _context.Users.Where(x => x.UserName == login).FirstOrDefaultAsync();

                if (password != null && BC.EnhancedVerify(oldPassword, password.Password, HashType.SHA512))
                {
                    password.Password = BC.EnhancedHashPassword(newPassword, 13, HashType.SHA512); ;

                    await _context.SaveChangesAsync();
                    return 200; // Password change successful
                }
                else
                {
                    return 400; // Incorrect old password
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the ChangePasswordAsync method.");
                return 500; // Unexpected error
            }
        }

        public async Task<int> ChangeEmailAsync(string login, string email)
        {
            try
            {
                var u_email = await _context.Users.Where(x => x.UserName == login).FirstOrDefaultAsync();

                if (u_email != null)
                {
                    u_email.Email = email;

                    await _context.SaveChangesAsync();
                }

                else
                {
                    return 400;
                }

                return 200;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the ChangeEmailAsync method.");
                return 500;
            }
        }
         // change it
        public async Task<IActionResult> SendNotificationAsync(string firstName, string lastName, string email, string phoneNumber, string message)
        {
            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

            if (Validators.IsEmailValid(email))
            {
                try
                {
                    smtpClient.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.Auto);
                    smtpClient.Authenticate(ApplicationDatas.FirstMail, ApplicationDatas.Password);

                    var localMessage = new MimeMessage();

                    localMessage.From.Add(new MailboxAddress("MyKahoot", ApplicationDatas.FirstMail));
                    localMessage.To.Add(new MailboxAddress("Me", "gurbanli.nidjat001@gmail.com"));

                    localMessage.Subject = "Feedback";

                    var part = new TextPart("plain")
                    {
                        Text = $"Feedback from {firstName} {lastName}\nUser mail: {email}\nUser phone number: {phoneNumber}\nUser feedback: {message}"
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
