using Audit.Realization.Configure;
using Audit.Realization.EntityFrameworkCore;
using Audit.Realization.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audit.Realization.Options
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureMyDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // 读取配置信息并将其绑定到 DatabaseOption 对象
            var appSettings = new DatabaseOption();
            configuration.GetSection("DatabaseOptions").Bind(appSettings);
            DatabaseConfig.InitConfiguration(appSettings);
            // 注册 HttpContextAccessor 服务
            services.AddHttpContextAccessor();
            // services.Configure<DatabaseOption>(configuration.GetSection("DatabaseOptions"));
            // 注册 MyDbContextConfigurationService 类
            services.AddScoped<IOptionsDbConfiguration, OptionsConfiguration>();
            switch (DatabaseConfig.DbType)
            {
                case DatabaseType.MySql:
                    services.AddDbContext<MySqlDbContext>();
                    break;
                case DatabaseType.SqlServer:
                    services.AddDbContext<SqlServerDbContext>();
                    break;
                case DatabaseType.Oracle:
                    services.AddDbContext<OracleDbContext>();
                    break;
                case DatabaseType.MongoDb:
                    services.AddSingleton(provider => new MongoDbContext(DatabaseConfig.ConnectionString, DatabaseConfig.DatabaseName));
                    break;
                default: throw new ArgumentException("未指定数据库类型");
            }
            //注册其它服务
            services.AddOtherExt();
            //验证表是否存在
            services.CreateTableConfig(DatabaseConfig.DbType);
        }

        /// <summary>
        /// 创建审计日志表
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void CreateTableConfig(this IServiceCollection services, DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.MySql:
                    services.CreateTable(type);
                    break;
                case DatabaseType.SqlServer:
                    services.CreateTable(type);
                    break;
                case DatabaseType.Oracle:
                    break;
                default: throw new ArgumentException("未指定数据库类型");
            }
        }
    }
}
