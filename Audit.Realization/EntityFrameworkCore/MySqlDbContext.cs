using Audit.Realization.Configure;
using Audit.Realization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Audit.Realization.EntityFrameworkCore
{
    /// <summary>
    /// Mysql
    /// </summary>
    public class MySqlDbContext : DbContext, IDynamicDbContext
    {
        public DbSet<AuditLog> AuditLogs { get; set; }
        public static readonly ILoggerFactory EFLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(DatabaseConfig.ConnectionString, new MySqlServerVersion(new Version(DatabaseConfig.Version ?? "8.0.22")));
            // 启用日志记录
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
            //判断数据库是否存在
            var isDatabase = Database.EnsureCreated();
            var tableExists = true;
            if (isDatabase)
            {
                var connection = Database.GetDbConnection();
                tableExists = connection.GetSchema("Tables", new[] { null, null, null, "TABLE" })
                    .Rows
                    .OfType<System.Data.DataRow>()
                    .Any(row => row["TABLE_NAME"].ToString().Equals("AuditLog", StringComparison.OrdinalIgnoreCase));


            }
            return tableExists;
        }
        /// <summary>
        /// 插入审计日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsertAsync(AuditLog entity)
        {
            await InsertAsync<AuditLog>(entity);
        }
        // 实现 IDynamicDbContext 中的 SaveChanges 方法
        private async Task InsertAsync<T>(T entity)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                await connection.OpenAsync();
                var tableName = typeof(T).Name; // 假设表名与实体类名相同
                var properties = typeof(T).GetProperties();

                var columnNames = string.Join(", ", properties.Select(p => p.Name));
                var columnValues = string.Join(", ", properties.Select(p => $"@{p.Name}"));

                var query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({columnValues})";

                using (var command = new MySqlCommand(query, connection))
                {
                    foreach (var prop in properties)
                    {
                        command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(entity) ?? DBNull.Value);
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
