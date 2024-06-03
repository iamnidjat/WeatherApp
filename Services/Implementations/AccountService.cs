using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Services.Interfaces;
using BCrypt.Net;
using BC = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using WeatherApp.Models;

namespace WeatherApp.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly WeatherAppDbContext _context;
        private readonly ILogger<AccountService> _logger;
        private readonly ApplicationDatas _appData;

        public AccountService(WeatherAppDbContext context, ILogger<AccountService> logger,
            ApplicationDatas appData)
        {
            _context = context;
            _logger = logger;
            _appData = appData;
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
         
        public async Task<IEnumerable<User>> GetSubscribedUsersAsync()
        {
            try
            {
                return await _context.Users.Where(u => u.IsSubscribed).ToListAsync();
            }
            catch (Exception ex) when (ex is ArgumentNullException or OperationCanceledException)
            {
                _logger.LogError(ex, "An error occurred in the GetSubscribedUsers method.");
                return Enumerable.Empty<User>();
            }
        }

        public async Task EnableSubscriptionAsync(int userId, string city)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    user.IsSubscribed = !user.IsSubscribed;
                    user.SubscribedCity = city;

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException or OperationCanceledException)
            {
                _logger.LogError(ex, "An error occurred in the EnableSubscription method.");
            }
        }

        public async Task DisableSubscriptionAsync(int userId, string city)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    user.IsSubscribed = !user.IsSubscribed;
                    user.SubscribedCity = "";

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException or OperationCanceledException)
            {
                _logger.LogError(ex, "An error occurred in the DisableSubscriptionAsync method.");
            }
        }

        public async Task<IActionResult> ResetPasswordAsync(string email)
        {
            var newPassword = RandomPasswordGenerator(RandomPasswordLength());

            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

            if (Validators.IsEmailValid(email))
            {
                try
                {
                    smtpClient.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.Auto);
                    smtpClient.Authenticate(_appData.FirstMail, _appData.Password);

                    var message = new MimeMessage();

                    message.From.Add(new MailboxAddress("WeatherApp", _appData.FirstMail));
                    message.To.Add(new MailboxAddress("You", email));

                    message.Subject = "Reset Password";

                    var part = new TextPart("plain")
                    {
                        Text = $"Your new password: {newPassword}\nIf the message was sent by mistake, just ignore it."
                    };

                    message.Body = part;

                    smtpClient.Send(message);

                    var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        user.Password = BC.EnhancedHashPassword(newPassword, 13, HashType.SHA512);

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return new StatusCodeResult(400);
                    }
                }
                catch (Exception ex) when (ex is InvalidOperationException or ArgumentNullException or InvalidCastException)
                {
                    _logger.LogError(ex, "An error occurred in the ResetPasswordAsync method.");
                    return new StatusCodeResult(500);
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

        private static int RandomPasswordLength() // No need for try catch here
        {
            var number = new Random().Next(5, 10);

            return number;
        }

        private static string RandomPasswordGenerator(int length)
        {
            //try
            //{
            byte[] result = new byte[length];

            for (int index = 0; index < length; index++)
            {
                result[index] = (byte)new Random().Next(33, 126);
            }

            return System.Text.Encoding.ASCII.GetString(result);
            //}
            //catch (Exception ex) when (ex is ArgumentException or ArgumentNullException or ArgumentOutOfRangeException)
            //{
            //    throw new Exception(ex.Message, ex);
            //}
        }
    }
}
