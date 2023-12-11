using Microsoft.AspNetCore.Http;

namespace Audit.Realization.Services
{
    public interface IUserInfoService
    {
        Task<(string? userId, string? userName)> GetUserInfoAsync(HttpContext httpContext);
    }
}
