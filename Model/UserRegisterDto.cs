namespace EmployeeAdminPortal.Model
{
    public class UserRegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool AcceptTerms { get; set; }
    }
}
