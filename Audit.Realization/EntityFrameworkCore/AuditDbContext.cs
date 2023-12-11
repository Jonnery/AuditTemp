using Audit.Realization.Configure;
using Audit.Realization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Audit.Realization.EntityFrameworkCore
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public class AuditDbContext : DbContext
    {
        public DbSet<AuditLog> AuditLogs { get; set; }
        /// <summary>
        /// 
        /// </summary>

        private readonly IConfiguration Configuration;

        /*public AuditDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
            DatabaseConfig.InitConfiguration(configuration);
        }*/

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var databaseType = Configuration.GetSection("DataBase:Dialect").Value;
                var connectionString = Configuration.GetConnectionString("Default");

                switch (databaseType?.ToLower())
                {
                    case "sqlserver":
                        optionsBuilder.UseSqlServer(DatabaseConfig.ConnectionString);
                        break;
                    case "mysql":
                        optionsBuilder.UseMySql(DatabaseConfig.ConnectionString, new MySqlServerVersion(new Version(DatabaseConfig.Version)));
                        break;
                    case "oracle":
                        optionsBuilder.UseOracle(DatabaseConfig.ConnectionString);
                        break;
                    // 添加其他数据库类型的支持
                    default:
                        throw new ArgumentException("Invalid database type specified in appsettings.json");
                }
            }
            //optionsBuilder.UseMySql(DatabaseConfig.ConnectionString, new MySqlServerVersion(new Version(DatabaseConfig.Version)));
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
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableExists(string tableName)
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
                    .Any(row => row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.OrdinalIgnoreCase));


            }
            return tableExists;
        }

        public void Insert(AuditLog entity)
        {
            Insert<AuditLog>(entity);
        }
        private void Insert<T>(T entity)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
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

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
