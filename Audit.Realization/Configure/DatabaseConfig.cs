using Audit.Realization.EntityFrameworkCore;
using Audit.Realization.Extensions;

namespace Audit.Realization.Configure
{
    /// <summary>
    /// 数据库配置信息转对象模型
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public static string? ConnectionString { get; private set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DatabaseType DbType { get; private set; } = DatabaseType.MySql;
        /// <summary>
        /// 版本,只有mysql的使用需要配置
        /// </summary>
        public static string? Version { get; private set; }

        /// <summary>
        /// mongodb的时候要给数据库名词
        /// </summary>
        public static string? DatabaseName { get; private set; }

        /// <summary>
        /// 用户认证地址
        /// </summary>
        public static string? UserInfoEndpointUrl { get; private set; }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="option"></param>
        public static void InitConfiguration(DatabaseOption option)
        {
            if (false) return;
            if (!option.DatabaseType.HasValue) return;
            ConnectionString = option.ConnectionString;
            DbType           = option.DatabaseType.Value;
            if (option.DatabaseType.Value == DatabaseType.Sqlite)
            {
                if (option.ConnectionString != null)
                {
                    ConnectionString = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, option.ConnectionString);
                }

            }
            DatabaseName        = option.DatabaseName;
            Version             = option.Version;
            UserInfoEndpointUrl = option.UserInfoEndpointUrl;

        }

        /*public static void InitConfiguration(IConfiguration configuration, IOptions<DatabaseOption>? option = null)
        {
            if (option != null)
            {
                var databaseOption = new DatabaseOption(); // 为了访问 DatabaseType 属性，确保 DatabaseOption 实例被正确初始化
                //option.Invoke(databaseOption);
                if (databaseOption.DatabaseType.HasValue)
                {
                    ConnectionString = databaseOption.ConnectionString;
                    DbType = databaseOption.DatabaseType.Value;
                    if (databaseOption.DatabaseType.Value == DatabaseType.Sqlite)
                    {
                        if(databaseOption.ConnectionString != null)
                        {
                            ConnectionString = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, databaseOption.ConnectionString);
                        }
                       
                    }
                    DatabaseName = databaseOption.DatabaseName;
                    Version = databaseOption.Version;
                }
                    
            }
            else
            {
                var dbTypeStr = configuration["DataBase:Dialect"];
                var connectString = configuration.GetConnectionString("Default");
                switch (dbTypeStr?.ToLower())
                {
                    case "mysql":
                        DbType = DatabaseType.MySql;
                        Version = configuration["Database:Version"];
                        break;

                    case "sqlserver":
                        DbType = DatabaseType.SqlServer;
                        break;

                    case "oracle":
                        DbType = DatabaseType.Oracle;
                        break;

                    case "sqlite":
                        DbType = DatabaseType.Sqlite;
                        connectString = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, connectString);
                        break;
                    case "mongodb":
                        DbType = DatabaseType.MongoDb;
                        DatabaseName = configuration["Database:DatabaseName"];
                        break;
                    default:
                        DbType = DatabaseType.Unknown;
                        break;
                }

                ConnectionString = connectString;
            }

        }*/
    }
}
