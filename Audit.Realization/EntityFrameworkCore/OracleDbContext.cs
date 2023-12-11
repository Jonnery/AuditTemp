using Audit.Realization.Configure;
using Audit.Realization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Drawing.Printing;

namespace Audit.Realization.EntityFrameworkCore
{
    /// <summary>
    /// Oracle数据库上下文
    /// </summary>
    public class OracleDbContext : DbContext, IDynamicDbContext
    {
        public DbSet<AuditLog> AuditLogs { get; set; }
        public static readonly ILoggerFactory EFLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 从 appsettings.json 中获取 SQL Server 连接字符串
            //var connectionString = "Your SQL Server Connection String"; // 从配置文件中读取

            optionsBuilder.UseOracle(DatabaseConfig.ConnectionString);
            //打印sql
            optionsBuilder.UseLoggerFactory(EFLoggerFactory);
            optionsBuilder.EnableSensitiveDataLogging(true);//显示sql参数
        }

        /// <summary>
        /// 根据实体创建表结构
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>().ToTable("AuditLog");

            base.OnModelCreating(modelBuilder);
        }
        /// <summary>
        /// 判断审计日志表是否存在
        /// </summary>
        /// <returns></returns>
        public bool TableExists()
        {
            var tableCount = 0;

            var sql = $"SELECT COUNT(*) FROM all_tables WHERE table_name = '{"AuditLog"}'";

            using (var connection = new OracleConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                using (var command = new OracleCommand(sql, connection))
                {
                    tableCount = Convert.ToInt32(command.ExecuteScalar());
                }
                connection.Close(); // 关闭连接
            }
            return tableCount > 0;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        public async Task InsertAsync(AuditLog entity)
        {
            await InsertAsync<AuditLog>(entity);
        }

        /// <summary>
        /// 构造执行语句并执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        private async Task InsertAsync<T>(T entity)
        {

            using (var connection = new OracleConnection(DatabaseConfig.ConnectionString))
            {
                 await connection.OpenAsync();
                var tableName = typeof(T).Name; // 假设表名与实体类名相同
                var properties = typeof(T).GetProperties();

                var columnNames = string.Join(", ", properties.Select(p => $"\"{p.Name}\""));
                var columnValues = string.Join(", ", properties.Select(p => $":{p.Name}"));

                var query = $"INSERT INTO \"AuditLog\" ({columnNames}) VALUES ({columnValues})";

                using (var command = new OracleCommand(query, connection))
                {
                    foreach (var prop in properties)
                    {
                        command.Parameters.Add(new OracleParameter($":{prop.Name}", prop.GetValue(entity) ?? DBNull.Value));
                    }

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="serviceName">服务名词</param>
        /// <param name="clientIpAddress">客户端IP</param>
        /// <returns></returns>
        public async Task<List<AuditLog>> GetByWhereAsync(string serviceName, string clientIpAddress, int pageIndex, int pageSize)
        {
            var query = AuditLogs.AsQueryable(); // 获取 IQueryable

            if (!string.IsNullOrEmpty(serviceName))
            {
                query = query.Where(log => log.ServiceName.Contains(serviceName));
            }

            if (!string.IsNullOrEmpty(clientIpAddress))
            {
                query = query.Where(log => log.ClientIpAddress.Contains(clientIpAddress));
            }

            // 计算要跳过的记录数
            var skipAmount = (pageIndex - 1) * pageSize;

            // 根据分页参数进行分页查询
            var result = await query.Skip(skipAmount).Take(pageSize).ToListAsync();

            return result;
        }

        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await AuditLogs.ToListAsync();
        }

    }
}
