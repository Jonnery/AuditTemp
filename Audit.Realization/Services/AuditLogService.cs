using Audit.Realization.Configure;
using Audit.Realization.DbContextFactory;
using Audit.Realization.Dto;
using Audit.Realization.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Audit.Realization.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly IDynamicDbContextFactory _dbContextFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public AuditLogService(IServiceProvider serviceProvider)
        {
            _dbContextFactory = serviceProvider.GetService<IDynamicDbContextFactory>();
        }

        /// <summary>
        /// 保存审计日志
        /// </summary>
        /// <param name="auditInfo"></param>
        /// <exception cref="ArgumentException"></exception>
        public async Task SaveAsync(AuditInfo auditInfo)
        {
            try
            {
                // 获取特定类型的数据库上下文
                var dbContext = _dbContextFactory.CreateDbContext(DatabaseConfig.DbType);
                if (dbContext != null)
                {
                    // 在此处执行数据库写入操作
                    var model = new AuditLog(Guid.NewGuid().ToString())
                    {
                        CustomData = auditInfo.CustomData,
                        ExecutionDuration = auditInfo.ExecutionDuration,
                        ServiceName = auditInfo.ServiceName,
                        BrowserInfo = auditInfo.BrowserInfo,
                        ClientIpAddress = auditInfo.ClientIpAddress,
                        ClientName = auditInfo.ClientName,
                        Exception = auditInfo.Exception != null ? auditInfo.Exception.ToString() : null,
                        ExecutionTime = auditInfo.ExecutionTime,
                        MethodName = auditInfo.MethodName,
                        Parameters = auditInfo.Parameters,
                        ReturnValue = auditInfo.ReturnValue,
                    };
                    await dbContext.InsertAsync(model);
                }
                else
                {
                    throw new ArgumentException("Get database context empty");
                }
            }
            catch (Exception ex)
            {

                Log.Information($"写入审计日志信息异常:{ex}");
            }
        }

        /// <summary>
        /// 分页查询审计日志
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="clientIpAddress">客户端IP</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<List<AuditLog>> GetByWhereListAsync(string serviceName, string clientIpAddress, int pageIndex = 1, int pageSize = 15)
        {
            var list = new List<AuditLog>();
            try
            {
                var dbContext = _dbContextFactory.CreateDbContext(DatabaseConfig.DbType);
                if (dbContext != null)
                {
                    list = await dbContext.GetByWhereAsync(serviceName, clientIpAddress, pageIndex, pageSize);
                }
                else
                {
                    throw new ArgumentException("Get database context empty");
                }
            }
            catch (Exception ex)
            {

                Log.Information($"查询审计日志信息异常:{ex}");
            }
            return list;
        }
    }

}
