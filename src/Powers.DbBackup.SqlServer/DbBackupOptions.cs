using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Smo;

namespace Powers.DbBackup.SqlServer
{
    public class DbBackupOptions : IOptions<DbBackupOptions>
    {
        /// <summary>
        /// 数据库地址
        /// </summary>
        public string ServerInstance { get; set; } = null!;

        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// 数据库密码
        /// </summary>

        public string Password { get; set; } = null!;

        /// <summary>
        /// 要备份的数据库
        /// </summary>
        public string DatabaseName { get; set; } = null!;

        /// <summary>
        /// 数据库导出设置
        /// </summary>
        public ScriptingOptions ScriptingOptions { get; set; } = new ScriptingOptions()
        {
            DriAll = true,
            ScriptSchema = true,
            ScriptData = true,
            ScriptDrops = false
        };

        /// <summary>
        /// 数据库表格匹配
        /// </summary>
        public IEnumerable<string> FormatTables { get; set; } = new List<string>();

        public DbBackupOptions Value => this;
    }
}