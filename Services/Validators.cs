using System;
using System.Text.RegularExpressions;

namespace WeatherApp.Services
{
    public class Validators
    {
        public static bool IsEmailValid(string email)
        {
            if (email != "")
            {
                string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|ru)$";

                return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
            }

            return false;
        }
    }
}
