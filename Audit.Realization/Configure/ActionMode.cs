using Audit.Realization.EntityFrameworkCore;
using Audit.Realization.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Audit.Realization.Configure
{
    /// <summary>
    /// 委托模式
    /// </summary>
    public static class ActionMode
    {
        public static void ConfigureDbContext(this IServiceCollection services, Action<DatabaseOption>? action = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddHttpContextAccessor();
            if (action != null)
            {
                var databaseOption = new DatabaseOption(); // 为了访问 DatabaseType 属性，确保 DatabaseOption 实例被正确初始化
                action?.Invoke(databaseOption);
                DatabaseConfig.InitConfiguration(databaseOption);
                if (databaseOption.DatabaseType.HasValue)
                {
                    switch (databaseOption.DatabaseType.Value)
                    {
                        case DatabaseType.MySql:
                            services.AddDbContext<MySqlDbContext>();
                            services.CreateTable(databaseOption.DatabaseType.Value);
                            break;
                        case DatabaseType.SqlServer:
                            services.AddDbContext<SqlServerDbContext>();
                            break;
                        case DatabaseType.MongoDb:
                            services.AddSingleton(provider => new MongoDbContext(databaseOption.ConnectionString, databaseOption.DatabaseName));
                            break;
                        case DatabaseType.Oracle:
                            services.AddDbContext<OracleDbContext>();
                            services.CreateTable(databaseOption.DatabaseType.Value);
                            break;

                    }
                }
                else
                {
                    throw new ArgumentException("Invalid database type specified in appsettings.json");
                }

            }

            //注册其它服务
            services.AddOtherExt();

        }
    }

}
