using Audit.Realization.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Audit.Realization.Models
{
    [Table("AuditLog")]
    public class AuditLog
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public AuditLog(string id)
        {
            Id = id;
        }
        /// <summary>
        /// 主键
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength250)]
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength50)]
        public string? UserName { get; set; }

        /// <summary>
        /// 服务 (类/接口) 名
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength250)]
        public string? ServiceName { get; set; }

        /// <summary>
        /// 执行方法名称
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength250)]
        public string? MethodName { get; set; }

        /// <summary>
        /// 调用参数
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength3000)]
        public string? Parameters { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength3000)]
        public string? ReturnValue { get; set; }

        /// <summary>
        /// 方法执行的开始时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// 方法调用的总持续时间（毫秒）
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        /// 客户端的IP地址
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength50)]
        public string? ClientIpAddress { get; set; }

        /// <summary>
        /// 客户端的名称（通常是计算机名）
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength100)]
        public string? ClientName { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength250)]
        public string BrowserInfo { get; set; }

        /// <summary>
        /// 方法执行期间发生异常
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength2000)]
        public string? Exception { get; set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        [MaxLength(EntityDefault.FieldsLength2000)]
        public string? CustomData { get; set; }
    }
}
