namespace EmployeeAdminPortal.Model
{
    public class MainMenu
    {
        public string Id { get; set; }
        public string ActionName { get; set; }
        public bool IsActive { get; set; }
        public string SystemID { get; set; }

        public List<SubMenu> SubMenus { get; set; } = new List<SubMenu>();
    }
}


