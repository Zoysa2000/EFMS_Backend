using System;
using System.Collections.Generic;
using System.Data;
using EmployeeAdminPortal.Model;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace EmployeeAdminPortal.Data
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public string AddEmployee(EmployeeDetails employee)
        {
            if (string.IsNullOrWhiteSpace(employee.Department))
                throw new ArgumentException("Department is required.");

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Lookup deptid and canonical deptname
                string deptId;
                string deptName;

                const string lookupSql = @"
            SELECT deptid, deptname
            FROM department
            WHERE UPPER(deptname) = :deptname";

                using (var lookupCmd = new OracleCommand(lookupSql, connection))
                {
                    lookupCmd.Transaction = transaction;
                    lookupCmd.Parameters.Add("deptname", OracleDbType.Varchar2).Value = employee.Department.ToUpper();

                    using var reader = lookupCmd.ExecuteReader();
                    if (!reader.Read())
                        throw new Exception($"Department '{employee.Department}' not found.");

                    deptId = reader.GetString(0);
                    deptName = reader.GetString(1);
                }

                
                const string insertSql = @"
            INSERT INTO Employee (
                deptid,
                first_name,
                last_name,
                department,
                phone,
                position,
                age,
                email,
                date_of_join,
                image_url,
                terms_accepted
            )
            VALUES (
                :deptid,
                :first_name,
                :last_name,
                :department,
                :phone,
                :position,
                :age,
                :email,
                :date_of_join,
                :image_url,
                :terms_accepted
            )
            RETURNING empid INTO :newEmpId";

                using var cmd = new OracleCommand(insertSql, connection);
                cmd.Transaction = transaction;

                cmd.Parameters.Add("deptid", OracleDbType.Varchar2).Value = deptId;
                cmd.Parameters.Add("first_name", OracleDbType.Varchar2).Value = employee.FirstName ?? (object)DBNull.Value;
                cmd.Parameters.Add("last_name", OracleDbType.Varchar2).Value = employee.LastName ?? (object)DBNull.Value;
                cmd.Parameters.Add("department", OracleDbType.Varchar2).Value = deptName;
                cmd.Parameters.Add("phone", OracleDbType.Varchar2).Value = employee.Phone ?? (object)DBNull.Value;
                cmd.Parameters.Add("position", OracleDbType.Varchar2).Value = employee.Position ?? (object)DBNull.Value;
                cmd.Parameters.Add("age", OracleDbType.Int32).Value = employee.Age;
                cmd.Parameters.Add("email", OracleDbType.Varchar2).Value = employee.Email ?? (object)DBNull.Value;
                cmd.Parameters.Add("date_of_join", OracleDbType.Date).Value = employee.DateOfJoin;
                cmd.Parameters.Add("image_url", OracleDbType.Varchar2).Value =
                    string.IsNullOrWhiteSpace(employee.ImageUrl) ? (object)DBNull.Value : employee.ImageUrl;
                cmd.Parameters.Add("terms_accepted", OracleDbType.Char).Value = employee.TermsAccepted ? "Y" : "N";

           
                var outParam = new OracleParameter("newEmpId", OracleDbType.Varchar2, 20)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                cmd.ExecuteNonQuery();
                transaction.Commit();

                return outParam.Value?.ToString();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

 public int UpdateEmployee(EmployeeDetails e)
        {
            using var con = new OracleConnection(_connectionString);
            con.Open();


            string deptId, deptName;

            const string lookupSql = @"
        SELECT deptid, deptname
        FROM department
        WHERE UPPER(deptname) = :deptname";

            using (var lookupCmd = new OracleCommand(lookupSql, con))
            {
                lookupCmd.Parameters.Add("deptname", OracleDbType.Varchar2).Value = e.Department.ToUpper();

                using var reader = lookupCmd.ExecuteReader();
                if (!reader.Read())
                    throw new Exception($"Department '{e.Department}' not found.");

                deptId = reader.GetString(0);     
                deptName = reader.GetString(1);   
            }

            const string updateSql = @"
        UPDATE Employee SET
            deptid         = :deptid,
            first_name     = :first_name,
            last_name      = :last_name,
            department     = :department,
            phone          = :phone,
            position       = :position,
            age            = :age,
            email          = :email,
            date_of_join   = :date_of_join,
            image_url      = :image_url,
            terms_accepted = :terms_accepted
        WHERE empid       = :empid";

            using var cmd = new OracleCommand(updateSql, con);

            cmd.Parameters.Add("deptid", OracleDbType.Varchar2).Value = deptId;
            cmd.Parameters.Add("first_name", OracleDbType.Varchar2).Value = e.FirstName ?? (object)DBNull.Value;
            cmd.Parameters.Add("last_name", OracleDbType.Varchar2).Value = e.LastName ?? (object)DBNull.Value;
            cmd.Parameters.Add("department", OracleDbType.Varchar2).Value = deptName; // use official name
            cmd.Parameters.Add("phone", OracleDbType.Varchar2).Value = e.Phone ?? (object)DBNull.Value;
            cmd.Parameters.Add("position", OracleDbType.Varchar2).Value = e.Position ?? (object)DBNull.Value;
            cmd.Parameters.Add("age", OracleDbType.Int32).Value = e.Age;
            cmd.Parameters.Add("email", OracleDbType.Varchar2).Value = e.Email ?? (object)DBNull.Value;
            cmd.Parameters.Add("date_of_join", OracleDbType.Date).Value = e.DateOfJoin;
            cmd.Parameters.Add("image_url", OracleDbType.Varchar2).Value =
                string.IsNullOrWhiteSpace(e.ImageUrl) ? (object)DBNull.Value : e.ImageUrl;
            cmd.Parameters.Add("terms_accepted", OracleDbType.Char).Value = e.TermsAccepted ? "Y" : "N";
            cmd.Parameters.Add("empid", OracleDbType.Varchar2).Value = e.EmpId;

            return cmd.ExecuteNonQuery();
        }





    public IEnumerable<EmployeeDetails> GetAllEmployees()
        {
            var employees = new List<EmployeeDetails>();

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string sql = @"
        SELECT empid, deptid, first_name, last_name, department, phone,
               position, age, email, date_of_join, image_url, terms_accepted
        FROM Employee
        ORDER BY empid";

            using var cmd = new OracleCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                employees.Add(new EmployeeDetails
                {
                    EmpId = reader.GetString(reader.GetOrdinal("empid")),
                    DeptId = reader.GetString(reader.GetOrdinal("deptid")),
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                    Department = reader.GetString(reader.GetOrdinal("department")),
                    Phone = reader.GetString(reader.GetOrdinal("phone")),
                    Position = reader.GetString(reader.GetOrdinal("position")),
                    Age = reader.GetInt32(reader.GetOrdinal("age")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    DateOfJoin = reader.GetDateTime(reader.GetOrdinal("date_of_join")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) ? string.Empty : reader.GetString(reader.GetOrdinal("image_url")),
                    TermsAccepted = reader.GetString(reader.GetOrdinal("terms_accepted")) == "Y"
                });
            }

            return employees;
        }



        public EmployeeDetails? GetEmployeeById(string empId)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string sql = @"
        SELECT empid,deptid, first_name, last_name, department, phone,
               position, age, email, date_of_join, image_url, terms_accepted
        FROM   Employee
        WHERE  empid = :empid";

            using var cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add(new OracleParameter("empid", empId));

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;      

            return new EmployeeDetails
            {
                EmpId = reader.GetString(reader.GetOrdinal("empid")),
                DeptId = reader.GetString(reader.GetOrdinal("deptid")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Department = reader.GetString(reader.GetOrdinal("department")),
                Phone = reader.GetString(reader.GetOrdinal("phone")),
                Position = reader.GetString(reader.GetOrdinal("position")),
                Age = reader.GetInt32(reader.GetOrdinal("age")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                DateOfJoin = reader.GetDateTime(reader.GetOrdinal("date_of_join")),
                ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url")),
                TermsAccepted = reader.GetString(reader.GetOrdinal("terms_accepted")) == "Y"
            };
        }


public IEnumerable<EmployeeDetails> GetEmployeesByDepartment(string department)
        {
            var employees = new List<EmployeeDetails>();

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            const string sql = @"
        SELECT empid, first_name, last_name, department, phone,
               position, age, email, date_of_join, image_url, terms_accepted
        FROM   Employee
        WHERE  UPPER(department) = :deptName
        ORDER  BY empid";

            using var cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add("deptName", OracleDbType.Varchar2).Value = department.ToUpper();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                employees.Add(new EmployeeDetails
                {
                    EmpId = reader.GetString(reader.GetOrdinal("empid")),
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                    Department = reader.GetString(reader.GetOrdinal("department")),
                    Phone = reader.GetString(reader.GetOrdinal("phone")),
                    Position = reader.GetString(reader.GetOrdinal("position")),
                    Age = reader.GetInt32(reader.GetOrdinal("age")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    DateOfJoin = reader.GetDateTime(reader.GetOrdinal("date_of_join")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url"))
                                   ? null
                                   : reader.GetString(reader.GetOrdinal("image_url")),
                    TermsAccepted = reader.GetString(reader.GetOrdinal("terms_accepted")) == "Y"
                });
            }

            return employees;
        }


        public EmployeeDetails? GetEmployeeByEmpIdAndDepartment(string empId, string deptName)
        {
            // Optional: whitelist if you still want to reject unknown departments
            var valid = new HashSet<string> { "HR", "SALES", "FINANCE", "IT", "MARKETING" };
            string upperDept = deptName.ToUpper();

            if (!valid.Contains(upperDept))
                throw new ArgumentException("Invalid department name");

            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            const string sql = @"
        SELECT empid, first_name, last_name, department, phone,
               position, age, email, date_of_join, image_url, terms_accepted
        FROM   Employee
        WHERE  empid = :empId
          AND  UPPER(department) = :deptName";

            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("empId", OracleDbType.Varchar2).Value = empId;
            cmd.Parameters.Add("deptName", OracleDbType.Varchar2).Value = upperDept;

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new EmployeeDetails
            {
                EmpId = r.GetString(r.GetOrdinal("empid")),
                FirstName = r.GetString(r.GetOrdinal("first_name")),
                LastName = r.GetString(r.GetOrdinal("last_name")),
                Department = r.GetString(r.GetOrdinal("department")),
                Phone = r.GetString(r.GetOrdinal("phone")),
                Position = r.GetString(r.GetOrdinal("position")),
                Age = r.GetInt32(r.GetOrdinal("age")),
                Email = r.GetString(r.GetOrdinal("email")),
                DateOfJoin = r.GetDateTime(r.GetOrdinal("date_of_join")),
                ImageUrl = r.IsDBNull(r.GetOrdinal("image_url"))
                               ? null
                               : r.GetString(r.GetOrdinal("image_url")),
                TermsAccepted = r.GetString(r.GetOrdinal("terms_accepted")) == "Y"
            };
        }





        public int DeleteEmployee(string deptName, string empId)
{
    using var connection = new OracleConnection(_connectionString);
    connection.Open();

    using var transaction = connection.BeginTransaction();

    try
            {
        string deptTable = deptName; 

        string deleteDeptSql = $@"
            DELETE FROM {deptTable}
            WHERE EMPID = :emp_id
              AND DEPT_NAME = :dept_name";

        using (var deleteDeptCmd = new OracleCommand(deleteDeptSql, connection))
        {
            deleteDeptCmd.BindByName = true;
            deleteDeptCmd.Parameters.Add("emp_id", OracleDbType.Varchar2).Value = empId;
            deleteDeptCmd.Parameters.Add("dept_name", OracleDbType.Varchar2).Value = deptName;
            deleteDeptCmd.Transaction = transaction;
            deleteDeptCmd.ExecuteNonQuery();
        }

        
        const string deleteEmpSql = @"
            DELETE FROM Employee
            WHERE EMPID = :emp_id
              AND DEPARTMENT = :dept_name";

        using (var deleteEmpCmd = new OracleCommand(deleteEmpSql, connection))
        {
            deleteEmpCmd.BindByName = true;
            deleteEmpCmd.Parameters.Add("emp_id", OracleDbType.Varchar2).Value = empId;
            deleteEmpCmd.Parameters.Add("dept_name", OracleDbType.Varchar2).Value = deptName;
            deleteEmpCmd.Transaction = transaction;

            int rows = deleteEmpCmd.ExecuteNonQuery();
            transaction.Commit();
            return rows; 
        }
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}





    }
}


