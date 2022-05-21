using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powers.DbBackup.SqlServer
{
    public static class DbBackupExtensions
    {
        private static DbBackupOptions _dbBackupOptions = null!;

        public static IServiceCollection AddDbBackup(this IServiceCollection services, Action<DbBackupOptions> opts)
        {
            opts.Invoke(_dbBackupOptions);
            if (_dbBackupOptions is null)
            {
                throw new ArgumentNullException(nameof(_dbBackupOptions), "请配置数据库备份配置");
            }

            return services;
        }

        /// <summary>
        /// 添加数据库备份相关方法
        /// </summary>
        /// <param name="services"> </param>
        /// <returns> </returns>
        /// <exception cref="ArgumentNullException"> </exception>
        public static IServiceCollection AddDbBackup(this IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>()!;
            var opt = configuration.GetSection("DbBackupOptions").Get<DbBackupOptions>();
            _dbBackupOptions = opt;
            if (_dbBackupOptions is null)
            {
                throw new ArgumentNullException(nameof(_dbBackupOptions), "请配置数据库备份配置");
            }
            return services;
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="dbBackupPath"> 备份根路径 </param>
        /// <param name="name">         文件名 </param>
        /// <returns> (备份路径, 备份大小) </returns>
        private static (string, string) BackupDatabaseSMO(string dbBackupPath, string name)
        {
            try
            {
                Server server = new Server(
                    new ServerConnection(
                        _dbBackupOptions.ServerInstance,
                        _dbBackupOptions.Username,
                        _dbBackupOptions.Password
                        )
                );
                Database templateDb = server.Databases[_dbBackupOptions.DatabaseName];

                string sqlFilePath = string.Format("{0}.sql", $"{dbBackupPath}/{name}");

                var startWith = _dbBackupOptions.FormatTables.Where(x => x.EndsWith("*")).Select(x => x.TrimEnd('*'));
                var endWith = _dbBackupOptions.FormatTables.Where(x => x.StartsWith("*")).Select(x => x.TrimStart('*'));

                if (_dbBackupOptions.FormatTables is not null && _dbBackupOptions.FormatTables.Any())
                {
                    foreach (Table tb in templateDb.Tables)
                    {
                        if (_dbBackupOptions.FormatTables.Contains(tb.Name) ||
                            startWith.Where(x => tb.Name.StartsWith(x)).Any() ||
                            endWith.Where(x => tb.Name.EndsWith(x)).Any())
                        {
                            IEnumerable<string> sqlStrs = tb.EnumScript(_dbBackupOptions.ScriptingOptions);
                            using (StreamWriter sw = new StreamWriter(sqlFilePath, true, Encoding.UTF8))
                            {
                                foreach (var sql in sqlStrs)
                                {
                                    sw.WriteLine(sql);
                                    sw.WriteLine("GO");
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (Table tb in templateDb.Tables)
                    {
                        IEnumerable<string> sqlStrs = tb.EnumScript(_dbBackupOptions.ScriptingOptions);
                        using (StreamWriter sw = new StreamWriter(sqlFilePath, true, Encoding.UTF8))
                        {
                            foreach (var sql in sqlStrs)
                            {
                                sw.WriteLine(sql);
                                sw.WriteLine("GO");
                            }
                        }
                    }
                }
                var fileInfo = new FileInfo(sqlFilePath);
                return (sqlFilePath, fileInfo.Length.ToString());
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);

                throw;
            }
        }

        /// <summary>
        /// 删除备份文件
        /// </summary>
        /// <param name="path"> 备份文件的路径 </param>
        /// <returns> </returns>
        public static async Task<(bool, string)> DeleteBackup(string path)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        return (false, "文件不存在");
                    }

                    File.Delete(path);
                    return (true, "删除成功");
                }
                catch (IOException err)
                {
                    return (false, err.Message);
                }
            });
        }

        /// <summary>
        /// 开始备份
        /// </summary>
        /// <param name="dbBackupPath"> 备份根路径 </param>
        /// <param name="name">         文件名 </param>
        /// <returns> (备份路径, 备份大小) </returns>
        public static async Task<(string, string)> StartBackupAsync(string dbBackupPath, string name)
        {
            return await Task.Run(() =>
            {
                return BackupDatabaseSMO(dbBackupPath, name);
            });
        }
    }
}