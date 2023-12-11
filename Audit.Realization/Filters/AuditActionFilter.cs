﻿using Audit.Realization.Auditing;
using Audit.Realization.Dto;
using Audit.Realization.Extensions;
using Audit.Realization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Audit.Realization.Filters
{
    public class AuditActionFilter : IAsyncActionFilter
    {
        private readonly IAuditLogService _auditLogService;
        //private readonly IAdmSession _admSession;
        private readonly ILogger<AuditActionFilter> _logger;
        private readonly IClientInfoProvider _clientInfoProvider;

        public AuditActionFilter(IAuditLogService auditLogService,
            //IAdmSession admSession,
            ILogger<AuditActionFilter> logger,
            IClientInfoProvider clientInfoProvider)
        {
            _auditLogService = auditLogService;
           // _admSession = admSession;
            _logger = logger;
            _clientInfoProvider = clientInfoProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldSaveAudit(context))
            {
                await next();
                return;
            }

            var type      = (context.ActionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.AsType();
            var method    = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            var arguments = context.ActionArguments;
            var stopwatch = Stopwatch.StartNew();

            var auditInfo = new AuditInfo
            {
                //UserId = _admSession?.UserId,
                //UserName = _admSession?.Name,
                ServiceName = type != null
                    ? type.FullName.TruncateWithPostfix(EntityDefault.FieldsLength250)
                    : "",
                //5.0版本以上，varchar(50)，指的是50字符，无论存放的是数字、字母还是UTF8汉字（每个汉字3字节），都可以存放50个。其他数据库要注意下这里
                MethodName = method.Name.TruncateWithPostfix(EntityDefault.FieldsLength250),
                Parameters = ConvertArgumentsToJson(arguments).TruncateWithPostfix(EntityDefault.FieldsLength2000),
                ExecutionTime = DateTime.Now,
                BrowserInfo = _clientInfoProvider.BrowserInfo.TruncateWithPostfix(EntityDefault.FieldsLength250),
                ClientIpAddress = _clientInfoProvider.ClientIpAddress.TruncateWithPostfix(EntityDefault.FieldsLength50),
                ClientName = _clientInfoProvider.ComputerName.TruncateWithPostfix(EntityDefault.FieldsLength100),
            };

            ActionExecutedContext result = null;
            try
            {
                result = await next();
                if (result.Exception != null && !result.ExceptionHandled)
                {
                    auditInfo.Exception = result.Exception;
                }
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                if (result != null)
                {
                    switch (result.Result)
                    {
                        case ObjectResult objectResult:
                            auditInfo.ReturnValue = JsonConvert.SerializeObject(objectResult.Value);
                            break;

                        case JsonResult jsonResult:
                            auditInfo.ReturnValue = JsonConvert.SerializeObject(jsonResult.Value);
                            break;

                        case ContentResult contentResult:
                            auditInfo.ReturnValue = contentResult.Content;
                            break;
                    }
                }
                Console.WriteLine(auditInfo.ToString());
                auditInfo.ReturnValue = auditInfo.ReturnValue.TruncateWithPostfix(EntityDefault.FieldsLength20);
                _auditLogService.SaveAsync(auditInfo);
            }
        }

        /// <summary>
        /// 是否开启审计
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static bool ShouldSaveAudit(ActionExecutingContext context, bool defaultValue = false)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor))
                return false;
            var methodInfo = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.HasAttribute<AuditedAttribute>())
            {
                return true;
            }

            if (methodInfo.HasAttribute<DisableAuditingAttribute>())
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.GetTypeInfo().HasAttribute<AuditedAttribute>())
                {
                    return true;
                }

                if (classType.GetTypeInfo().HasAttribute<DisableAuditingAttribute>())
                {
                    return false;
                }
            }
            return defaultValue;
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments)
                {
                    dictionary[argument.Key] = argument.Value;
                }

                return JsonConvert.SerializeObject(dictionary);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString(), ex);
                return "{}";
            }
        }
    }
}
