using Audit.Realization.EntityFrameworkCore;

namespace Audit.Realization.DbContextFactory
{
    /// <summary>
    /// 数据库上下文工厂接口层
    /// </summary>
    public interface IDynamicDbContextFactory
    {
        IDynamicDbContext CreateDbContext(DatabaseType dbType);

    }
}
