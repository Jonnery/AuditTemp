using Audit.Realization.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Audit.Realization.Extensions
{
    public static class CreateTableService
    {
        /// <summary>
        /// MySql创建表结构
        /// </summary>
        /// <param name="services"></param>
        public static void CreateTable(this IServiceCollection services, DatabaseType databaseType)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            switch (databaseType)
            {
                case DatabaseType.MySql:
                {
                    var serviceProvider = services.AddDbContext<MySqlDbContext>()
                                                  .BuildServiceProvider();
                    using var dbContext = serviceProvider.GetService<MySqlDbContext>();
                    // 判断表是否存在
                    var tableExists = dbContext.TableExists();

                    if (!tableExists)
                    {
                        using var scope        = serviceProvider.CreateScope();
                        var       dbContextStr = scope.ServiceProvider.GetRequiredService<MySqlDbContext>();
                        dbContext.Database.EnsureCreated();
                    }

                    break;
                }
                case DatabaseType.SqlServer:
                {
                    var serviceProvider = services.AddDbContext<SqlServerDbContext>()
                                                  .BuildServiceProvider();
                    using var dbContext = serviceProvider.GetService<SqlServerDbContext>();
                    // 判断表是否存在
                    var tableExists = dbContext.TableExists();

                    if (!tableExists)
                    {
                        using var scope        = serviceProvider.CreateScope();
                        var       dbContextStr = scope.ServiceProvider.GetRequiredService<SqlServerDbContext>();
                        dbContext.Database.EnsureCreated();
                    }

                    break;
                }
                case DatabaseType.Oracle:
                {
                    var serviceProvider = services.AddDbContext<OracleDbContext>()
                                                  .BuildServiceProvider();
                    using var dbContext = serviceProvider.GetService<OracleDbContext>();
                    // 判断表是否存在
                    var tableExists = dbContext.TableExists();

                    if (!tableExists)
                    {
                        using var scope        = serviceProvider.CreateScope();
                        var       dbContextStr = scope.ServiceProvider.GetRequiredService<OracleDbContext>();
                        // 手动应用模型配置
                        dbContext.Database.EnsureCreated(); // 会创建数据库，但不会执行迁移
                        dbContext.Database.Migrate();
                        dbContext.Database.OpenConnection(); // 打开连接以应用模型配置
                        //dbContext.Database.ExecuteSqlRaw(@"CREATE TABLE AuditLog\r\n(\r\n    Id VARCHAR2(250) NOT NULL,\r\n    UserId VARCHAR2(250),\r\n    UserName VARCHAR2(50),\r\n    ServiceName VARCHAR2(250),\r\n                                                                                         VARCHAR2(250),\r\n    Parameters VARCHAR2(2000),\r\n    ReturnValue VARCHAR2(2000),\r\n    ExecutionTime TIMESTAMP,\r\n    ExecutionDuration NUMBER,\r\n    ClientIpAddress VARCHAR2(50),\r\n    ClientName VARCHAR2(100),\r\n    BrowserInfo VARCHAR2(250),\r\n    Exception VARCHAR2(2000),\r\n    CustomData VARCHAR2(2000),\r\n    CONSTRAINT PK_AuditLog PRIMARY KEY (Id)\r\n);"); // 执行自定义 SQL 命令来创建表格或应用其他模型配置
                        dbContext.Database.ExecuteSqlRaw("CREATE TABLE \"AuditLog\" (\r\n\t\"Id\" VARCHAR2 ( 250 ) NOT NULL ENABLE,\r\n\t\"UserId\" VARCHAR2 ( 250 ),\r\n\t\"UserName\" VARCHAR2 ( 50 ),\r\n\t\"ServiceName\" VARCHAR2 ( 250 ),\r\n\t\"MethodName\" VARCHAR2 ( 250 ),\r\n\t\"Parameters\" VARCHAR2 ( 3000 ),\r\n\t\"ReturnValue\" VARCHAR2 ( 3000 ),\r\n\t\"ExecutionTime\" TIMESTAMP ( 7 ),\r\n\t\"ExecutionDuration\" NUMBER ( 10, 0 ) ,\r\n\t\"ClientIpAddress\" VARCHAR2 ( 50 ),\r\n\t\"ClientName\" VARCHAR2 ( 100 ),\r\n\t\"BrowserInfo\" VARCHAR2 ( 250 ),\r\n\t\"Exception\" VARCHAR2 ( 2000 ),\r\n\t\"CustomData\" VARCHAR2 ( 2000 ),\r\n\tCONSTRAINT \"SYS_C0023708\" PRIMARY KEY ( \"Id\" )\r\n);");
                        dbContext.Database.CloseConnection(); // 关闭连接
                    }

                    break;
                }
            }
        }
    }
}
