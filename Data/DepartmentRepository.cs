using EmployeeAdminPortal.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace EmployeeAdminPortal.Data
{
    public class DepartmentRepository
    {
        private readonly string _connectionString;

        public DepartmentRepository(IConfiguration cfg)
        {
            _connectionString = cfg.GetConnectionString("OracleDb");
        }

   
        public IEnumerable<Department> GetAllDept()
        {
          
            const string sql = @"
                SELECT deptid,
                       deptname,
                       isactive
                FROM   Department
                ORDER  BY deptid";

            var list = new List<Department>();

            using var con = new OracleConnection(_connectionString);
            con.Open();

            using var cmd = new OracleCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Department
                {
                    DeptId = reader.GetString(0),         
                    DeptName = reader.GetString(1),       
                    IsActive = reader.GetInt32(2) == 1       
                });
            }

            return list;
        }
    }
}

