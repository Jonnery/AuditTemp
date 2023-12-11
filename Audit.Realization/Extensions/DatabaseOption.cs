using Audit.Realization.EntityFrameworkCore;

namespace Audit.Realization.Extensions
{
    public class DatabaseOption
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public string? ConnectionString { get; set; } = string.Empty;
        /// <summary>
        /// mongodb的时候要给数据库名词
        /// </summary>
        public string? DatabaseName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType? DatabaseType { get; set; }

        /// <summary>
        /// Mysql的时候指定数据库版本
        /// </summary>
        public string? Version { get; set; }

        public string? UserInfoEndpointUrl { get; private set; }
    }
}
