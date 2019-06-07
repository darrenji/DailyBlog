> 认证服务器上的Authorizaiton Endpoint用来验证用户身份， Token Endpoint用来发放Access Token从而访问受保护API资源， UerInfo Endpoint用来获取用户信息。在Authorization Code Flow中，首先会Redirect到认证服务器的一个登陆界面，当用户输入用户名和密码，在同意页面做出选择，点击确定之后，认证服务器就对用户进行验证，并且发送Authorization Code给浏览器，浏览器再拿着这个Authrization Code向认证服务器的Token Endpoint发送请求，获取到Acess Token。

比如在一个Web应用程序的Startup.cs中大致这样：

```
services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    ......
                    options.ResponseType = "code";
                    ......
                });
```

> 而在Hybrid Flow中，`options.ResponseType`的值可能是`code id_token`,`code token`, `code id_token token`。区别在于，当认证服务器对用户身份认证授权后返回给浏览器的就是这3种可能性。需要知道的是，浏览器和认证服务器的Authorization Endpoint交互的时候都发生在浏览器本身，在这以后，在和认证服务器的Token Endpoint交互时，是应用程序服务器和认证服务器之间的交互。

## 验证服务器，端口5000

- Config.cs
```
// mvc, hybrid flow
                new Client
                {
                    ClientId = "hybrid client",
                    ClientName = "ASP.NET Core Hybrid client",
                    ClientSecrets= { new Secret("hybrid_secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.Hybrid,
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
                        IdentityServerConstants.StandardScopes.Phone
                    }
                }
```
- Startup.cs
```
var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddTestUsers(TestUsers.Users);

            // in-memory, code config
            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiResources(Config.GetApis());
            builder.AddInMemoryClients(Config.GetClients());

            // in-memory, json config
            //builder.AddInMemoryIdentityResources(Configuration.GetSection("IdentityResources"));
            //builder.AddInMemoryApiResources(Configuration.GetSection("ApiResources"));
            //builder.AddInMemoryClients(Configuration.GetSection("clients"));

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }


            app.UseIdentityServer();
```
## API服务器，端口50001

略

## 应用程序，端口7001

- Startup.cs
```
 public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //如果没有这句，会出现Jwt的claims,有了这句，出现well-known的claims,容易辨识的claim
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "hybrid client";
                    options.ClientSecret = "hybrid_secret";
                    options.SaveTokens = true;
                    options.ResponseType = "code id_token";

                    options.Scope.Clear();
                    options.Scope.Add("api1");
                    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    options.Scope.Add(OidcConstants.StandardScopes.Profile);
                    options.Scope.Add(OidcConstants.StandardScopes.Email);
                    options.Scope.Add(OidcConstants.StandardScopes.Phone);
                    options.Scope.Add(OidcConstants.StandardScopes.Address);
                    options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);

                    //把一些被自动过滤掉的claim找回来
                    options.ClaimActions.Remove("nbf");
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.Remove("exp");

                    //删除一些不需要的claim
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("sub");
                    options.ClaimActions.DeleteClaim("idp");
                });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();

            //app.UseMvc();
            app.UseMvcWithDefaultRoute();
        }
    }
```

- HomeController.cs

```
[Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();

            //获取文档
            var disc = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if(disc.IsError)
            {
               
                throw new Exception(disc.Error);
            }

            //获取access token
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            //请求受保护API资源
            client.SetBearerToken(accessToken);
            var resposne =await client.GetAsync("http://localhost:50001/identity");
            if(!resposne.IsSuccessStatusCode)
            {
                if(resposne.StatusCode == HttpStatusCode.Unauthorized)
                {
                    accessToken = await RenewTokenAsync();
                    return RedirectToAction();
                }
                throw new Exception(resposne.ReasonPhrase);
            }
            var content = await resposne.Content.ReadAsStringAsync();
            return View("Index", content);
        }

        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var authorizationCode = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Code);

            ViewData["accessToken"] = accessToken;
            ViewData["idToken"] = idToken;
            ViewData["refreshToken"] = refreshToken;


            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        private async Task<string> RenewTokenAsync()
        {
            var client = new HttpClient();

            //请求文档
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if(disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            //获取当前refresh token
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            //拿着当前的refresh token重新获取token
            var tokenResposne = await client.RequestRefreshTokenAsync(new RefreshTokenRequest {
                Address = disco.TokenEndpoint,
                ClientId = "hybrid client",
                ClientSecret= "hybrid_secret",
                Scope= "api1 openid profile email phone address",
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                RefreshToken = refreshToken
            });

            if(tokenResposne.IsError)
            {
                throw new Exception(tokenResposne.Error);
            }

            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResposne.ExpiresIn);

            var token = new[] {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = tokenResposne.IdentityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResposne.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResposne.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };

            //重新验证
            var currentAuthenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            currentAuthenticateResult.Properties.StoreTokens(token);

            //重新登陆
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

            return tokenResposne.AccessToken;
        }
    }
```