> Authorizaton Code Flow,比如用在MVC应用程序，其实MVC也有自己的验证授权，通过Authorizaton Code Flow把验证授权交给IdentityServer4。既然OpenId Connect是在OAuth基础上的封装，这就有大致如下的相关性。

OAuth2.0 | OpenId Connect
------------ | -------------
Authorization Code Grant | Authorization Code Flow
Implicit Grant | Implicit Flow
空   | Hybrid Flow
Resource Owner Password Credential Grant | Password Credential Flow
Client Credential Grant | Client Credential Flow

> 在Authorizaton Code Flow中，用户在界面登录，并且在授权同意页面选择同意或者不同意，整个过程包含了身份请求和Token请求。假设验证服务器的端口是5000， Web网站的端口是5002，交互细节大致如下：

当用户输入用户名密码，在同一授权页面做选择，会向验证服务器发送身份请求。

```
GET 5000/connect/authorize:response_type=code
    &scope=xxx
    &client_id=xxx
    &state=xxx
    &redirect_uri=xxx

因为请求中包含了redirect_uri,所以服务器会把响应发给web服务器。

5002/signin-oidc
    code=xxx
    state=xxx

浏览器收到code实际上通过开发者工具是可以捕获到的，但对User来说，其实是看不到的，对User来说获取code就是一个临时步骤，接下来Web应用程序会带着这个code向验证服务器发送一个token请求。
```

说得简单一点，当用户输入用户名密码，在同意授权页做选择后发起身份请求，得到的来自验证服务器的code，接着就向验证服务器请求Token,带着这个刚刚拿到的code。Web应用程序的Token请求如下：

```
POST 5000/connect/token
    client_id
    client_secret
    code
    grant_type=authorization_code,
    redirect_uri

然后验证服务器返回给web服务器的包括：access_token, id_token, refresh_token, token_type=Bearer。这些信息被放在了浏览器的Cookie中。
```

## 验证服务器  5000

- Config.cs
```
public static class Config
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new IdentityResource[]{
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new IdentityResources.Phone(),
            new IdentityResources.Email()
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return new ApiResource[]{
            new ApiResource("api1", "");
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new[]{
            new Client{
                ClientId = "mvc client",
                ClientName = "",
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                ClientSecret = {new Secret("mvc_secret".Sha256())},
                RedirectUris = {"http://localhost:5002/signin-oidc"},//验证服务器向这个地址发code
                FrontChannelLogoutUri="http://localhost:5002/signout-oidc",//登出
                PostLogoutRedirectUris={"http://localhost:5002/signout-callback-oidc"},//登出的回调，可能是回调验证服务器的
                AlwaysIncludUserClaimsInIdToken=true,
                AllowOfflineAccess=true,//refresh token
                AllowedScopes={
                    "api1",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Phone
                }
            }
        };
    }
}
```

- AccountOptons.cs
```
public class AccountOptions
{
    public static bool AutomaticRedirectAfterSignOut = true;//登出后跳转到web页，而不是验证服务器页
}
```
- Startup.cs
```
public class Startup
{
    public void ConfigureServices(IServieCollection services)
    {
        var builder = services.AddIdentityServer(options => {
            options.Events.RaiseErrorEvetns = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
        })
        .AddTestUsers(TestUsers.Users);

        builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
        builder.AddInMemoryApiResources(Config.GetApis());
        builder.AddInMemeoryClients(Config.GetClients());

        if(Environment.IsDevelopment())
        {
            builder.AddDeveloperSigningCredential();
        }
        else
        {
            throw new Exception("need to configure key material");
        }
    }

    public Configute(IApplicationBuilder app)
    {
        app.UseIdentityServer();
    }
}
```

## API资源 50001


- Startup.cs
```
services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.Authority = "http://localhost:5000";
        options.RequireHttpsMetadata = false;
        options.Audience = "api1";
    })

app.UseAuthentication();
```

## Web服务器 5002

- Startup.cs
```
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

services.AddAuthetnication(options =>{
    options.DefaultScheme = CookieAuthenticaitonDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticaitonDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticaitonScheme;
        options.Authority = "http://localhost:5000";
        options.RequireHttpsMetadata = false;
        options.ClientId = "mvc client";
        options.ClientSecret = "mvc_secret";
        options.SaveTokens = true;
        options.ResponseType = "code";

        options.Scope.Clear();
        options.Scope.Add("api1");
        options.Scope.Add(OidcConstants.StandardScopes.OpenId);
        options.Scope.Add(OidcConstants.StandardScopes.Profile);
        options.Scope.Add(OidcConstants.StandardScopes.Email);
        options.Scope.Add(OidcConstants.StandardScopes.Address);
        options.Scope.Add(OidcConstants.StandardScopes.Phone);
        options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);

    })

app.UseAuthentication();
```
- HomeController.cs

```
public class HomeController : Controller
{
    public async Task<IActionResult> Index()
    {
        //先文档
        var client = new HttpClient();
        var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
        if(disco.IsError)
        {
            throw new Exception(disco.Error);
        }

        //获取token
        var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectionParameterNames.AccessToken);
        client.SetBearerToken(accessToken);

        //请求
        var response = await client.GetAsync("http://localhost:50001/identity");

        if(!response.IsSuccessStatusCode)
        {
            throw new Exception(response.ReasonPhrase);
        }

        var content = await response.Content.ReadAsStringAsync();

        return View("Index", content);
    }

    public async Task<IActionResult> Privacy()
    {
        var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        var idToken = await HttpContext.GetAsync(OpenIdConnectParameterNames.IdToken);
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
}
```