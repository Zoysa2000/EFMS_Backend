using Oracle.ManagedDataAccess.Client;
using EmployeeAdminPortal.Model;
using Microsoft.Extensions.Configuration;

namespace EmployeeAdminPortal.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public void AddUser(UserRegisterDto user)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();
            Console.WriteLine("Oracle connection opened.");

            string sql = @"INSERT INTO users (EMAIL, PASSWORD, ACCEPT_TERMS) 
                   VALUES (:email, :password, :acceptTerms)";

         
            string hashedPassword = PasswordHelper.HashPassword(user.Password);

            using var cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add(new OracleParameter("email", user.Email));
            cmd.Parameters.Add(new OracleParameter("password", hashedPassword));
            cmd.Parameters.Add(new OracleParameter("acceptTerms", user.AcceptTerms ? 1 : 0));

            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine($"{rows} row(s) inserted.");
        }


        public bool ValidateUserCredentials(string email, string password)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            string sql = "SELECT PASSWORD FROM users WHERE EMAIL = :email";

            using var cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add(new OracleParameter("email", email));

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string storedHashedPassword = reader.GetString(0);
                string inputHashedPassword = PasswordHelper.HashPassword(password);
                return storedHashedPassword == inputHashedPassword;
            }

            return false;
        }

        public bool ChangePassword(string email, string newPassword)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            string sql = "UPDATE users SET PASSWORD = :password WHERE EMAIL = :email";

            string hashedPassword = PasswordHelper.HashPassword(newPassword);

            using var cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add(new OracleParameter("password", hashedPassword));
            cmd.Parameters.Add(new OracleParameter("email", email));

            int rowsUpdated = cmd.ExecuteNonQuery();

            return rowsUpdated > 0; // true if password was updated
        }






    }
}
