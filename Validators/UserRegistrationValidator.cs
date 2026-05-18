using System.Text.RegularExpressions;
using EmployeeAdminPortal.Model;

namespace EmployeeAdminPortal.Validators
{
    public static class UserRegistrationValidator
    {
        public static (bool IsValid, string Message) Validate(UserRegisterDto userDto)
        {
            if (userDto == null)
                return (false, "User data is required.");

            if (string.IsNullOrWhiteSpace(userDto.Email) ||
                string.IsNullOrWhiteSpace(userDto.Password) ||
                string.IsNullOrWhiteSpace(userDto.ConfirmPassword))
            {
                return (false, "Email and Passwords are required.");
            }

            if (!IsValidEmail(userDto.Email))
            {
                return (false, "Invalid email format.");
            }

            if (userDto.Password != userDto.ConfirmPassword)
            {
                return (false, "Passwords do not match.");
            }

            if (userDto.Password.Length < 6)
            {
                return (false, "Password must have at least 6 characters.");
            }

            if (!userDto.AcceptTerms)
            {
                return (false, "You must accept the terms.");
            }

            return (true, "Validation successful.");
        }
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }
    }
}


