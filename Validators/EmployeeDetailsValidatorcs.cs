using System;
using System.Text.RegularExpressions;
using EmployeeAdminPortal.Model;

namespace EmployeeAdminPortal.Validators
{
    public static class EmployeeDetailsValidator
    {
        public static (bool IsValid, string Message) Validate(EmployeeDetails employee)
        {
            if (employee == null)
                return (false, "Employee data is required.");

            
            if (string.IsNullOrWhiteSpace(employee.FirstName))
                return (false, "First name is required.");

            if (string.IsNullOrWhiteSpace(employee.LastName))
                return (false, "Last name is required.");

            if (string.IsNullOrWhiteSpace(employee.Department))
                return (false, "Department is required.");

            if (string.IsNullOrWhiteSpace(employee.Phone))
                return (false, "Phone number is required.");
            if (!IsValidPhone(employee.Phone))
                return (false, "Phone number format is invalid.");

            if (string.IsNullOrWhiteSpace(employee.Position))
                return (false, "Position is required.");

            if (employee.Age < 18 || employee.Age > 65)
                return (false, "Age must be between 18 and 65.");

            if (string.IsNullOrWhiteSpace(employee.Email))
                return (false, "Email is required.");
            if (!IsValidEmail(employee.Email))
                return (false, "Email format is invalid.");

            if (employee.DateOfJoin == default)
                return (false, "Date of join is required.");

            

            return (true, "Validation successful.");
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        private static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Allows digits, optional +, 7 to 15 digits total
            var phonePattern = @"^\+?\d{7,15}$";
            return Regex.IsMatch(phone, phonePattern);
        }
    }
}
