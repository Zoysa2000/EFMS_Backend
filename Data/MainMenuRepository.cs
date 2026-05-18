using EmployeeAdminPortal.Model;
using Oracle.ManagedDataAccess.Client;

namespace EmployeeAdminPortal.Data
{
    public class MainMenuRepository
    {
        private readonly string _connectionString;

        public MainMenuRepository(IConfiguration cfg)
        {
            _connectionString = cfg.GetConnectionString("OracleDb");
        }

        public IEnumerable<MainMenu> GetMenus()
        {
            const string sql = @"
                SELECT 
                    m.id, m.action_name, m.active, m.system_id,
                    s.submenu_id, s.component_name, s.display_name, s.display_order, s.active
                FROM mainmenu m
                LEFT JOIN submenu s ON m.id = s.mainmenu_id
                WHERE m.system_id = 'S001'
                ORDER BY m.id, s.display_order";

            var menus = new Dictionary<string, MainMenu>();

            using var con = new OracleConnection(_connectionString);
            con.Open();

            using var cmd = new OracleCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var mainMenuId = reader.GetString(0);

                if (!menus.ContainsKey(mainMenuId))
                {
                    menus[mainMenuId] = new MainMenu
                    {
                        Id = mainMenuId,
                        ActionName = reader.GetString(1),
                        IsActive = reader.GetInt32(2) == 1,
                        SystemID = reader.GetString(3),
                        SubMenus = new List<SubMenu>()
                    };
                }

                if (!reader.IsDBNull(4)) 
                {
                    menus[mainMenuId].SubMenus.Add(new SubMenu
                    {
                        SubMenuId = reader.GetString(4),
                        MainMenuId = mainMenuId,
                        ComponentName = reader.GetString(5),
                        DisplayName = reader.GetString(6),
                        DisplayOrder = reader.GetInt32(7),
                        IsActive = reader.GetInt32(8) == 1
                    });
                }
            }

            return menus.Values;
        }
    }
}
