using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using System.Security.Claims;
using System.Security.Principal;
using System.Linq;
using System.Collections.Concurrent;

[assembly: OwinStartup(typeof(WebApi.Startup))]

namespace WebApi
{
    public class Startup
    {
        private readonly ConcurrentDictionary<string, string> _authenticationCodes = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                /*
                 * 客户端应用可以直接访问并得到访问令牌的地址， 必须以前倒斜杠 "/" 开始，
                 * 例如： /Token ， 如果想客户端颁发了 client_secret ， 那么客户端必须将其发送到这个地址；
                */
                TokenEndpointPath = new PathString("/Token"),
                /*
                 * 客户端应用将用户浏览器重定向到用户同意颁发令牌或代码的地址， 必须以前倒斜杠 "/" 开始，
                 * 例如： /Authorize ；
                 */
                AuthorizeEndpointPath = new PathString("/Account/Authorize"),
                /*
                 * 应用程序提供和 OAuth 认证中间件交互的 IOAuthAuthorizationServerProvider 实例，
                 * 通常可以使用默认的 OAuthAuthorizationServerProvider ， 并设置委托函数即可。
                 */
                Provider = new OAuthAuthorizationServerProvider
                {
                    //OnValidateClientRedirectUri = ValidateClientRedirectUri,
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                    //OnGrantClientCredentials = GrantClientCredentials
                },
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                ApplicationCanDisplayErrors = true,
                AllowInsecureHttp = true,

                AuthorizationCodeProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateAuthenticationCode,
                    OnReceive = ReceiveAuthenticationCode,
                },

                //RefreshTokenProvider = new AuthenticationTokenProvider
                //{
                //    OnCreate = CreateRefreshToken,
                //    OnReceive = ReceiveRefreshToken,
                //}
                /*
				•AuthorizeEndpointPath : 客户端应用将用户浏览器重定向到用户同意颁发令牌或代码的地址， 必须以前倒斜杠 "/" 开始， 例如： /Authorize ；
				•TokenEndpointPath : 客户端应用可以直接访问并得到访问令牌的地址， 必须以前倒斜杠 "/" 开始， 例如： /Token ， 如果想客户端颁发了 client_secret ， 那么客户端必须将其发送到这个地址；
				•ApplicationCanDisplayErrors : 如果希望在 /Authorize 这个地址显示自定义错误信息， 则设置为 true ， 只有当浏览器不能被重定向到客户端时才需要， 比如 client_id 和 redirect_uri 不正确； /Authorize 节点可以通过提取添加到 OWIN 环境的 oauth.Error 、 oauth.ErrorDescription 和 oauth.ErrorUri 属性来显示错误； 如果设置为 false ， 客户端浏览器将会被重定向到默认的错误页面；
				•AllowInsecureHttp : 如果允许客户端的 return_uri 参数不是 HTTPS 地址， 则设置为 true 。
				•Provider : 应用程序提供和 OAuth 认证中间件交互的 IOAuthAuthorizationServerProvider 实例， 通常可以使用默认的 OAuthAuthorizationServerProvider ， 并设置委托函数即可。
				•AuthorizationCodeProvider : 提供返回给客户端能且只能使用一次的认证码， 出于安全性考虑， OnCreate/OnCreateAsync 生成的认证码必须只能在 OnReceive/OnReceiveAsync 使用一次；
				•RefreshTokenProvider : 刷新令牌， 如果这个属性没有设置， 则不能从 /Token 刷新令牌。
				*/
            });
        }

        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext obj)
        {
            obj.DeserializeTicket(obj.Token);
        }

        private void CreateRefreshToken(AuthenticationTokenCreateContext obj)
        {
            obj.SetToken(obj.SerializeTicket());
        }

        private void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext obj)
        {
            string value;
            if (_authenticationCodes.TryRemove(obj.Token, out value))
            {
                obj.DeserializeTicket(value);
            }
        }

        private void CreateAuthenticationCode(AuthenticationTokenCreateContext obj)
        {
            obj.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            _authenticationCodes[obj.Token] = obj.SerializeTicket();
        }

        private Task GrantClientCredentials(OAuthGrantClientCredentialsContext arg)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(
                arg.ClientId,
                OAuthDefaults.AuthenticationType
                ),
                arg.Scope.Select(key => new Claim("urn:oauth:scope", key))
                );
            arg.Validated(identity);

            return Task.FromResult(0);
        }

        private Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext arg)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(
                arg.UserName,
                OAuthDefaults.AuthenticationType
                ),
                arg.Scope.Select(key => new Claim("urn:oauth:scope", key))
                );
            arg.Validated(identity);

            return Task.FromResult(0);

        }

        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext arg)
        {
            if (arg.ClientId == null)
            {
                arg.Validated();
            }
            //string clientId;
            //string clientSecret;
            //if (arg.TryGetBasicCredentials(out clientId, out clientSecret) || arg.TryGetFormCredentials(out clientId, out clientSecret))
            //{
            //    if (clientId == "user1" && clientSecret == "user1Password")
            //    {
            //        arg.Validated();
            //    }
            //}
            return Task.FromResult(0);
        }

        private Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext arg)
        {
            if (arg.ClientId == "user1")
            {
                arg.Validated("redirecturi");
            }
            return Task.FromResult(0);
        }

    }
}
