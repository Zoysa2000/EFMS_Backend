namespace EmployeeAdminPortal.Model
{
    public class SubMenu
    {
        public string SubMenuId { get; set; }
        public string MainMenuId { get; set; }
        public string ComponentName { get; set; }
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

