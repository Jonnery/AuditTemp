using Audit.Realization.Models;

namespace Audit.Realization.EntityFrameworkCore
{
    public interface IDynamicDbContext
    {
        bool TableExists();
        /// <summary>
        /// 插入审计日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertAsync(AuditLog entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName">服务名词</param>
        /// <param name="clientIpAddress">客户端IP</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<AuditLog>> GetByWhereAsync(string serviceName, string clientIpAddress, int pageIndex =1, int pageSize = 10);
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        Task<List<AuditLog>> GetAllAsync();
    }

}
