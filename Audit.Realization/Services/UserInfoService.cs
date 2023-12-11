using Audit.Realization.Configure;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Audit.Realization.Services
{
    /*public class UserInfoService
    {
        private readonly ClaimsIdentity _claimsIdentity;
        private string? _userId;
        private string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _claimsIdentity = httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
        }

        public string? UserId
        {
            get
            {
                if (_userId != null)
                    return _userId;
                var claim = _claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                return claim == null ? null : (_userId = claim.Value.ToString());
            }
        }


        public string UserName => _claimsIdentity.Name;

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(_name))
                    return _name;
                return _name = _claimsIdentity.FindFirst(JwtClaimTypes.Name)?.Value;
            }
        }
    }*/

    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string? userId, string? userName)> GetUserInfoAsync(HttpContext httpContext)
        {
            var authScheme = await GetCurrentAuthenticationScheme(httpContext);
            switch (authScheme)
            {
                case "Bearer":
                    return GetUserInfoFromJwt(httpContext);

                case "OAuth2":
                    return await GetUserInfoFromOAuth2(httpContext);

                case "OpenIDConnect":
                    return await GetUserInfoFromOpenIDConnect(httpContext);

                case "IdentityServer":
                    return await GetUserInfoFromIdentityServer(httpContext);

                // 其他认证方案的处理
                // case "OtherAuthScheme":
                //     return GetUserInfoFromOtherAuthScheme(httpContext);

                default:
                    return (null, null);
            }
        }

        private async Task<string?> GetCurrentAuthenticationScheme(HttpContext httpContext)
        {
            var result = await httpContext.AuthenticateAsync();
            return result?.Principal?.Identity?.AuthenticationType;
        }

        private (string? userId, string? userName) GetUserInfoFromJwt(HttpContext httpContext)
        {
            var user = httpContext.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = user.Identity?.Name;
            return (userId, userName);
        }

        private async Task<(string? userId, string? userName)> GetUserInfoFromOAuth2(HttpContext httpContext)
        {
            // 实现 OAuth2 方式获取用户信息的逻辑
            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return (null, null);
            }

            var userInfoEndpoint = DatabaseConfig.UserInfoEndpointUrl;//"https://your-oauth2-provider.com/userinfo"; // 你的 OAuth2 Provider 的 UserInfo Endpoint

            var client = new System.Net.Http.HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync(userInfoEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // 处理从 OAuth2 Provider 获取到的用户信息
                // 可能需要将 JSON 解析为对象，获取其中的用户 ID 和用户名等信息
                // 示例：
                // var userInfo = JObject.Parse(content);
                // var userId = userInfo["sub"]?.ToString();
                // var userName = userInfo["preferred_username"]?.ToString();
                // return (userId, userName);
            }
            return (null, null);
        }

        private async Task<(string? userId, string? userName)> GetUserInfoFromOpenIDConnect(HttpContext httpContext)
        {
            // 实现 OpenID Connect 方式获取用户信息的逻辑
            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return (null, null);
            }

            var userInfoEndpoint = DatabaseConfig.UserInfoEndpointUrl;//"https://your-openidconnect-provider.com/userinfo"; // OpenID Connect Provider 的 UserInfo Endpoint

            var client = new System.Net.Http.HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync(userInfoEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // 处理从 OpenID Connect Provider 获取到的用户信息
                // 可能需要将 JSON 解析为对象，获取其中的用户 ID 和用户名等信息
                // 示例：
                // var userInfo = JObject.Parse(content);
                // var userId = userInfo["sub"]?.ToString();
                // var userName = userInfo["preferred_username"]?.ToString();
                // return (userId, userName);
            }
            return (null, null);
        }

        private async Task<(string? userId, string? userName)> GetUserInfoFromIdentityServer(HttpContext httpContext)
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return (null, null);
            }

            var userInfoEndpoint = DatabaseConfig.UserInfoEndpointUrl;//"https://your-identityserver.com/connect/userinfo"; // 你的 IdentityServer 的 UserInfo Endpoint

            var client = new System.Net.Http.HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync(userInfoEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // 处理从 IdentityServer 获取到的用户信息
                // 可能需要将 JSON 解析为对象，获取其中的用户 ID 和用户名等信息
                // 示例：
                // var userInfo = JObject.Parse(content);
                // var userId = userInfo["sub"]?.ToString();
                // var userName = userInfo["preferred_username"]?.ToString();
                // return (userId, userName);
            }

            return (null, null);
        }
    }
}
