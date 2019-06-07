> JWT token信息自包含，无法直接进行生命周期控制。Reference Token也是一种身份标识，在认证服务器有一个Token IntroSpection Endpoint,可以直接控制token的生命周期，缺点是与认证服务器通信很频繁，优点是安全性高。

在验证服务器的ApiResource方面需要配置API的密码。

```
        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1", new List<string>{ "location"}){
                    ApiSecrets = { new Secret("api1_secret".Sha256())} //此处为Reference Token设置
                } //把claim带到API资源中去
            };
        }
```

验证服务器的Client有一个属性需要设置:`AccessTokenType = AccessTokenType.Reference`

```
                new Client
                {
                    ClientId = "hybrid client",
                    ClientName = "ASP.NET Core Hybrid client",
                    ClientSecrets= { new Secret("hybrid_secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.Hybrid,
                    AccessTokenType = AccessTokenType.Reference, //改变默认的JWT授权方式，此处为Reference Token设置
                    RedirectUris =
                    {
                        "http://localhost:7000/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "http://localhost:7000/signout-callback-oidc"
                    },
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes =
                    {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "roles",
                        "locations"
                    }
                }
```

API所在的Web需要安装组件`IdentityServer4.AccessTokenValidation`，然后在`Startup.cs`中配置如下：

```
services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => {
                    options.Authority = "http://localhost:5000";
                    options.ApiName = "api1";
                    options.RequireHttpsMetadata = false;
                    options.ApiSecret = "api1_secret";
                });
```

## 撤销token

在web端程序中：

```
        public async Task Logout()
        {
            #region reference token
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
            if(disco.IsError)
            {
                throw new Exception(disco.Error);
            }


            //吊销access token
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if(!string.IsNullOrWhiteSpace(accessToken))
            {
                
                var revokeAccessTokenResposne = await client.RevokeTokenAsync(new TokenRevocationRequest {
                    Token = accessToken,
                    Address = disco.RevocationEndpoint,
                    ClientId = "hybrid client",
                    ClientSecret = "hybrid_secret"
                });

                if(revokeAccessTokenResposne.IsError)
                {
                    throw new Exception("Access Token Revocation Failed:" + revokeAccessTokenResposne.Error);
                }
            }

            //吊销reference token
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            if(!string.IsNullOrWhiteSpace(refreshToken))
            {
                var revokeRefreshTokenResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Token = refreshToken,
                    Address = disco.RevocationEndpoint,
                    ClientId= "hybrid client",
                    ClientSecret= "hybrid_secret"
                });

                if(revokeRefreshTokenResponse.IsError)
                {
                    throw new Exception("Refresh Token Revocation Failed:" + revokeRefreshTokenResponse.Error);
                }
            }

            #endregion

            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
```