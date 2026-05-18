using Oracle.ManagedDataAccess.Client;

namespace EmployeeAdminPortal.Data
{
    public class CountRepository
    {
        private readonly string _connectionString;

        public CountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public int GetEmployeeCount()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM Employee";

            using var command = new OracleCommand(query, connection);
            var result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        public int GetEmployeeCountIT()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM Employee WHERE deptid = 'D002'";

            using var command = new OracleCommand(query, connection);
            var result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        public int GetEmployeeCountHR()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM Employee WHERE deptid = 'D001'";

            using var command = new OracleCommand(query, connection);
            var result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }
        public int GetEmployeeCountFinance()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM Employee WHERE deptid = 'D004'";
            using var command = new OracleCommand(query, connection);
            var result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }
        public int GetEmployeeCountSales()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string query = "SELECT COUNT(*) FROM Employee WHERE deptid = 'D003'";

            using var command = new OracleCommand(query, connection);
            var result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }




    }
}
