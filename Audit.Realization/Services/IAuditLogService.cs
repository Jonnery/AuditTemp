using Audit.Realization.Dto;
using Audit.Realization.Models;

namespace Audit.Realization.Services
{
    public interface IAuditLogService
    {
        /// <summary>
        /// 保存审计信息
        /// </summary>
        /// <param name="log"></param>
        Task SaveAsync(AuditInfo log);

        /// <summary>
        /// 分页查询审计日志
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="clientIpAddress">客户端IP</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        Task<List<AuditLog>> GetByWhereListAsync(string serviceName, string clientIpAddress, int pageIndex = 1, int pageSize = 15);
    }
}
