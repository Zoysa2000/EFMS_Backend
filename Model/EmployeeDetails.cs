namespace EmployeeAdminPortal.Model
{
    public class EmployeeDetails
    {
        public string EmpId { get; set; } = string.Empty;
        public string DeptId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfJoin { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool TermsAccepted { get; set; }


        // List to hold multiple contribution records
        public List<Pdf> Contributions { get; set; } = new List<Pdf>();
    }
}



