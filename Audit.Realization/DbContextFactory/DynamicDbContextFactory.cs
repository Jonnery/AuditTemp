using Audit.Realization.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Audit.Realization.DbContextFactory
{
    /// <summary>
    /// 数据库上下文工厂
    /// </summary>
    public class DynamicDbContextFactory : IDynamicDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DynamicDbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDynamicDbContext CreateDbContext(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.MongoDb:
                    return _serviceProvider.GetService<MongoDbContext>();
                case DatabaseType.SqlServer:
                    return _serviceProvider.GetService<SqlServerDbContext>();
                case DatabaseType.Oracle:
                    return _serviceProvider.GetService<OracleDbContext>();
                case DatabaseType.MySql:
                    return _serviceProvider.GetService<MySqlDbContext>();
                default:
                    return _serviceProvider.GetService<MySqlDbContext>();
            }
        }
    }
}
