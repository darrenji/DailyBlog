> 基于角色

## 验证服务器

- 用户,添加新的claim:  `new Claim(JwtClaimTypes.Role, "管理员")`
```
public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser{SubjectId = "818727", Username = "alice", Password = "alice", 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim(JwtClaimTypes.Role, "管理员")
                }
            },
            new TestUser{SubjectId = "88421113", Username = "bob", Password = "bob", 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "somewhere"),
                    new Claim(JwtClaimTypes.Role, "普通用户")

                }
            }
        };
    }
```
- Config.cs, 添加一个IdentityResource, 添加一个scope
```
public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource("roles","角色", new List<string>{ JwtClaimTypes.Role}) //这里的roles指scope
            };
        }

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
                        IdentityServerConstants.StandardScopes.Phone,
                        "roles"
                    }
                }
```


## web服务器

- Startup.cs, 在cookies中配置授权失败的转向页，在OpenIdConnect中配置有关roles
```
services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.AccessDeniedPath = "/Authorization/AccessDenied";
                })
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
                    options.Scope.Add("roles");

                    //把一些被自动过滤掉的claim找回来
                    options.ClaimActions.Remove("nbf");
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.Remove("exp");

                    //删除一些不需要的claim
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("sub");
                    options.ClaimActions.DeleteClaim("idp");

                    //有关用户的roles
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role
                    };
                });
```

- 在控制器中

```
[Authorize(Roles = "管理员,普通用户")]
```

> 基于策略，将多个claim组合在一起

在Startup.cs中的`services.AddAuthorization`中配置策略

```
//配置策略
            services.AddAuthorization(options => {
                options.AddPolicy("SmithInSomewhere", builder => {
                    builder.RequireAuthenticatedUser();
                    builder.RequireClaim(JwtClaimTypes.FamilyName, "Smith"); //这里值可以有多个，逗号分隔
                    builder.RequireClaim("location", "somewhere"); //这个不是标准claim哦
                });
            });
```

这里自定义的claim叫做location,位于某个scope中，这个scope需要在`services.AddAuthentication`中加上。
```
services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.AccessDeniedPath = "/Authorization/AccessDenied";
                })
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
                    options.Scope.Add("roles");
                    options.Scope.Add("locations");

                    //把一些被自动过滤掉的claim找回来
                    options.ClaimActions.Remove("nbf");
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.Remove("exp");

                    //删除一些不需要的claim
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("sub");
                    options.ClaimActions.DeleteClaim("idp");

                    //有关用户的roles
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role
                    };
                });
```
以上的locations这个scope需要在验证服务器上设置。

```
public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource("roles","角色", new List<string>{ JwtClaimTypes.Role}), //这里的roles指scope
                new IdentityResource("locations","地点", new List<string>{ "location"} )
            };
        }

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
                        IdentityServerConstants.StandardScopes.Phone,
                        "roles",
                        "locations"
                    }
                }
```

## 更复杂的策略

- 一个action可以有多个Policy作用
- 一个Policy, 通过`RequireAuthenticatedUser`,`RequireClaim`, `IAuthorizationRequirement`生成，
- 一个`IAuthorizationRequirement`中有多个`AuthorizationHandler`

首先需要实现`IAuthorizationRequirement`

```
public class SmithInSomewhereRequirement : IAuthorizationRequirement
    {
        public SmithInSomewhereRequirement()
        {

        }
    }
```

其次需要一个`AuthorizationHandler`

```
    public class SmithInSomewhereHandler : AuthorizationHandler<SmithInSomewhereRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SmithInSomewhereRequirement requirement)
        {
            //var filterContext = context.Resource as AuthorizationFilterContext;
            //if(filterContext==null)
            //{
            //    context.Fail();
            //    return Task.CompletedTask;
            //}

            var familyName = context.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value;
            var location = context.User.Claims.FirstOrDefault(t => t.Type == "location")?.Value;

            if(familyName == "Smith" && location == "somewhere" && context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
```

最后在`startup.cs`中设置：

```
//配置策略
services.AddAuthorization(options => {
    //options.AddPolicy("SmithInSomewhere", builder => {
    //    builder.RequireAuthenticatedUser();
    //    builder.RequireClaim(JwtClaimTypes.FamilyName, "Smith"); //这里值可以有多个，逗号分隔
    //    builder.RequireClaim("location", "somewhere"); //这个不是标准claim哦
    //});

    options.AddPolicy("SmithInSomewhere", builder => {
        builder.AddRequirements(new SmithInSomewhereRequirement());
    });
});
```

