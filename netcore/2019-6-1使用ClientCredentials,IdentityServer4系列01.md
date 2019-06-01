> ClientCredentials适用于软件没有用户交互的情况，比如无人值守的应用程序。

在IdentityServer4中的几个核心角色是：

- User
- Client: 编写的应用程序
- IdentityServer4本身
- Resources: API Resources, IdentityResources

IdentityServer4的几个核心类包括：

TestUser.cs
```
SubjectId
Username
Password
Claims
```

Client.cs
```
ClientId:
ClientName
AllowedGrantTypes:在这里设置交互类型
ClientSecrets:
AllowedScopes

//以下web应用程序中用到
RedirectUris
FrontChannelLogoutUri
PostLogoutRedirectUris
AllowOfflineAccess

//以下单页面应用程序中用到
ClientUri
AccessTokensViaBrowser
AllowedCorsOrigins
```

ApiResources.cs
IdentityResources.cs

## 脚手架模板

```
查看现有的模板：dotnet new
添加有关IdentityServer4的模板：dotnet new -i IdentityServer4.Templates
取消IdentityServer4的模板：dotnet new --debug:reinit
```

## IdentityServer4认证服务器

- 根据模板创建：dotnet new is4inmem --name Idp
- 用vs打开并保存解决保存
- Config.cs
```
public static class Config
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new IdentityResource[]{
            new IdentityResources.OpenId(),
            new IdentityResoruces.Profile()
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
                ClientId="console client",
                ClientName = "",
                AllowedGrantTypes=GrantTypes.ClientCredentials,
                ClientSecrets={ new Secret("my_secret".shar256())},
                AllowedScopes={"api1"}
            }
        };
    }
}
```
- Startup.cs

```
public class Startup
{
    public IHostingEnvironment Environment{get;}
    public IConfiguration Configuration{get;}

    pubic Startup(IHostingEnvironment environment, IConfiguration configuraiton)
    {
        Environment = environment;
        Configuration = configuraiton;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

        //添加IdnetityServer相关
        var builder = services.AddIdentityServer(options => {
            options.Events.RaiseErrorEvents = true;
            optiosn.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
        })
        .AddTestUsers(TestUsers.Users);

        builder.AddInMemeroryIdentityResources(Config.GetIdnentityResource());
        builder.AddInMemoryApiResources(Config.GetApis());
        builder.AddInMemroryClients(Config.GetClients());

        if(Environment.IsDevelopment())
        {
            builder.AddDeveloperSigningCredential();
        }
        else
        {
            throw new Exception("need to configure key material");
        }

        //如果有第三方验证
        services.AddAuthentication()
            .AddGoogle(options => {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.CLientId = "copy client d from google here";
                options.ClientSecret= "copy client secret from google here"
            });
    }

    publci void Configure(IApplicationBuilder app)
    {
        if(Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        //IdentityServer4相关
        app.UseIentityServer();
        app.UseStaticFiles();
        app.UseMvcWithDefaultRoute();
    }
}
```


## Web程序提供API

- 根据模板创建：dotnet new web --name API1Resource
- Startup.cs
```
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //告诉需要认证
        services.AddMvcCore()
            .AddAuthorization()
            .AddJsonFormatters();

        //告诉需要到验证服务器
        services.AddAuthenticaiton("Bearer")
            .AddJwtBearer("Bearer", options => {

                options.Authority = "http://localhost:5000";
                options.ReaurieHttpsMetadata = false;
                options.Audience = "api1";
            })
    }

    public void Configure(ApplicaitonBuilder app, IHostingEnvironment env)
    {
        app.UseAuthenticaiton();
        app.UseMvc();
    }
}
```
- IdentityController
```
[Route("identity")]
[Authorize]
public class IdentityController : ControllerBase
{
    public IActionResult Get()
    {
        var result = from c in User.Claims
            select new {c.Type, c.Value};
        return new JsonResult(result);
    }
}
```

## 控制台客户端

- 根据模板创建：dotnet new console --name ConsoleClient
- 引入IdentityModel数据包
```
static async Task Main(string[] args)
{
    var client = new HttpClient();

    //请求文档
    var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
    if(disco.IsError)
    {
        Console.WriteLine(disco.Error);
        return;
    }

    //获取token
    //ClientCrededentialsTokenRequest
    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest{
        Address = deisco.TokenEndpoint,
        ClientId = "console client",
        ClientSecret = "my_secret",
        Scope = "api1";
    });
    
    if(tokenResponseIsError)
    {
        Console.WriteLein(tokenResponse.Error);
        return;
    }

    //使用access token
    var apiClient = new HttpClient();
    apiClient.SetBearerToken(tokenResponse.AccessToken);

    var response = await apiClient.GetAsync(http://localhost:5001/identity);
    if(!response.IsSuccessStatusCode)
    {
        Console.WriteLine(response.StatusCode);
    }
    else
    {
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JArray.Parse(content));
    }

    Console.ReadKey();
}
```
- 控制台请求如果用Fiddler捕获

```
grant_type=client_credetials&client_id=xxx&client_secret=xxx&scope=xxx
```