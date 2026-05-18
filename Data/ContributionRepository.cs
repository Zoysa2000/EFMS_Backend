using EmployeeAdminPortal.Model;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace EmployeeAdminPortal.Data
{
    public class ContributionRepository
    {
        private readonly string _connectionString;

        public ContributionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task SaveContributionsAsync(List<Contribution> contributions)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            foreach (var c in contributions)
            {
                using var cmd = new OracleCommand(@"
            INSERT INTO Contributions (EMPID, ContributionDate, ContributionAmount)
            VALUES (:EMPID, :ContributionDate, :ContributionAmount)", connection);

              
                cmd.Parameters.Add(new OracleParameter("EMPID", OracleDbType.Varchar2)).Value = c.EMPID;
                cmd.Parameters.Add(new OracleParameter("ContributionDate", OracleDbType.Date)).Value = c.ContributionDate;
                cmd.Parameters.Add(new OracleParameter("ContributionAmount", OracleDbType.Decimal)).Value = c.ContributionAmount;

                await cmd.ExecuteNonQueryAsync();
            }
        }

    }
}
