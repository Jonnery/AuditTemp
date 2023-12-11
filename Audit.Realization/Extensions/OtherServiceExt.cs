using Audit.Realization.Auditing;
using Audit.Realization.DbContextFactory;
using Audit.Realization.Filters;
using Audit.Realization.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Audit.Realization.Extensions
{
    /// <summary>
    /// 其它的服务
    /// </summary>
    public static class OtherServiceExt
    {

        public static void AddOtherExt(this IServiceCollection services)
        {
            // 注册 DynamicDbContextFactory 类
            services.AddScoped<IDynamicDbContextFactory, DynamicDbContextFactory>();
            services.AddSingleton<IClientInfoProvider, HttpContextClientInfoProvider>();

            //services.AddSingleton<HttpContextClientInfoProvider>();
            services.AddControllers(option =>
            {
                option.Filters.Add(typeof(AuditActionFilter));
            }).AddNewtonsoftJson(option =>
            {
                //忽略循环引用
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });


            services.AddScoped<IAuditLogService, AuditLogService>();
        }
    }
}
