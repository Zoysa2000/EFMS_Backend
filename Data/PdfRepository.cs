using EmployeeAdminPortal.Model;
using Oracle.ManagedDataAccess.Client;

namespace EmployeeAdminPortal.Data
{
    public class PdfRepository
    {

        private readonly string _connectionString;

        public PdfRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public EmployeeDetails? GetEmployeeDetailsById(string empId)
        {
            EmployeeDetails? employee = null;

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            string sql = @"
                SELECT
                    e.EMPID,
                    e.DEPTID,
                    e.FIRST_NAME,
                    e.LAST_NAME,
                    e.DEPARTMENT,
                    e.PHONE,
                    e.POSITION,
                    e.AGE,
                    e.EMAIL,
                    e.DATE_OF_JOIN,
                    e.IMAGE_URL,
                    e.TERMS_ACCEPTED,
                    c.CONTRIBUTIONDATE,
                    c.CONTRIBUTIONAMOUNT
                FROM
                    EMPLOYEE e
                LEFT JOIN
                    CONTRIBUTIONS c ON e.EMPID = c.EMPID
                WHERE
                    e.EMPID = :empId
                ORDER BY
                    c.CONTRIBUTIONDATE DESC";

            using var command = new OracleCommand(sql, connection);
            command.Parameters.Add(new OracleParameter("empId", empId));

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                // If employee object is null, create and fill basic info from first row
                if (employee == null)
                {
                    employee = new EmployeeDetails
                    {
                        EmpId = reader["EMPID"] == DBNull.Value ? string.Empty : reader["EMPID"].ToString(),
                        DeptId = reader["DEPTID"] == DBNull.Value ? string.Empty : reader["DEPTID"].ToString(),
                        FirstName = reader["FIRST_NAME"] == DBNull.Value ? string.Empty : reader["FIRST_NAME"].ToString(),
                        LastName = reader["LAST_NAME"] == DBNull.Value ? string.Empty : reader["LAST_NAME"].ToString(),
                        Department = reader["DEPARTMENT"] == DBNull.Value ? string.Empty : reader["DEPARTMENT"].ToString(),
                        Phone = reader["PHONE"] == DBNull.Value ? string.Empty : reader["PHONE"].ToString(),
                        Position = reader["POSITION"] == DBNull.Value ? string.Empty : reader["POSITION"].ToString(),
                        Age = reader["AGE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AGE"]),
                        Email = reader["EMAIL"] == DBNull.Value ? string.Empty : reader["EMAIL"].ToString(),
                        DateOfJoin = reader["DATE_OF_JOIN"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["DATE_OF_JOIN"]),
                        ImageUrl = reader["IMAGE_URL"] == DBNull.Value ? string.Empty : reader["IMAGE_URL"].ToString(),
                        TermsAccepted = reader["TERMS_ACCEPTED"] != DBNull.Value &&
                    reader["TERMS_ACCEPTED"].ToString()?.Trim().ToUpper() == "Y",
                        Contributions = new List<Pdf>()
                    };
                }

                // Add contribution record if exists (can be null if no contributions)
                if (reader["CONTRIBUTIONDATE"] != DBNull.Value && reader["CONTRIBUTIONAMOUNT"] != DBNull.Value)
                {
                    var contribution = new Pdf
                    {
                        ContributionDate = Convert.ToDateTime(reader["CONTRIBUTIONDATE"]),
                        ContributionAmount = Convert.ToDecimal(reader["CONTRIBUTIONAMOUNT"])
                    };
                    employee.Contributions.Add(contribution);
                }
            }

            return employee;
        }
    }
}
