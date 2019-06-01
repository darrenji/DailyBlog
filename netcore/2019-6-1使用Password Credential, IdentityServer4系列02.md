> 全称是Resource Owner Password Credential, 用户在应用程序界面上输入用户名和密码向验证服务器请求access token，这种方式适用于客户端应用程序和资源所有者完全信任, 一般用在遗留系统请求验证服务器，这种方式不太常见，一般不用。

## 验证服务器

- Config.cs

```
public static class Config
{
    publis tatic IEnumerable<IdentityResource> GetIdentityResources()
    {
        //这里提供的IdentityResources资源需要在Client的定义中开放给Client
        return new IdentityResources[]{
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new IdentityResources.Phone(),
            new IdentityResources.Email()
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return enw ApiResource[]{
            new ApiResource("api1","")
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new[]
        {
            new Client{
                ClientId = "wpf client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = {new Secret("wpf_secret".sha256())},
                AllowdScopes = {
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
- Startup.cs

```
public void ConfigureServices(IServiceCollection services)
{
    var builder = services.AddIdentityServer(options => {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })
    .AddTestUsers(TestUsers.Users);

    builder.AddInMemoryIdentityResource(Config.GetIdentityResources());
    builder.AddInMemoryApiResources(Config.GetApis());
    buidler.AddInMemoryClients(Config.GetClients());

    if(Environment.IsDevelopment())
    {
        builder.AddDeveloperSigningCredential();
    }
    else
    {

    }

    //验证服务器向其它验证服务器发出验证请求
    services.AddAuthentication()
        .AddGoogle(
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            options.ClientId = "copy client id from google";
            options.ClientSecret = "copy client secret from google ";
        );
}

public void Configute(IApplicationBuilder app)
{
    app.UseIdentityServer();
    app.UseStaticFiles();
    app.UseMvcWithDefaultRoute();
}
```
- localhost:5000

## API所在的web应用程序

- Startup.cs
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvcCore()
        .AddAuthorization()
        .AddJsonFormatters();

    services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options => {
            options.Authority = "http://localhost:5000";
            options.RequrieHttpsMetadata = false;
            options.Audeience = "api1";
        })
}
```
- IdentityController.cs
```
[Route("identity")]
[Authorize]
public class IdentityController : ControllerBase
{
    public IActionResult Get()
    {
        var temp = from c in User.Claims
            select new {c.Type, c.Value};
        return new JsonResult(temp);
    }
}
```
- localhost:50001

## WPF客户端

公共部分
```
private string _accessToken;
private DiscoveryResponse _disco;
```

请求access token的按钮
```
var userName
var password

var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
_disco = disco;

if(disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest{
    Address = disco.TokenEndpoint,
    ClientId = "wpf client",
    ClientSecret = "wpf_secret",
    Scope="api1 openid profile address phone email",
    UserName=userName,
    Password=password
});

if(tokenResponse.IsError)
{
    MessageBox.Show(tokenResponse.Error);
    return;
}

_accessToken = tokenRespone.AccessToken;
AccessTokenTextBlock.Text = tokenResponse.Json.ToString();
```

请求API资源的按钮
```
private async void Button_Click_1(object sender, ROutedEventArgs e)
{
    var apiClient = new HttpClient();
    apiClient.SetBearerToken(_accessToken);

    var response = await apiClient.GetAsync("http://localhost:50001/identity");
    if(!response.IsSuccesStatusCode)
    {

    }
    else
    {
        var content = await response.Content.ReadAsStringAsync();
        Api1ResponseTextBlock.Text = content;
    }
}
```

请求IdentityResource资源的按钮
```
private async void ButtonClick_2(object sender, RoutedEventArgs e)
{
    var apiClient = new HttpClient();
    apiClient.SetBearerToken(_accessToken);

    var response = await apiClient.GetAsync(_disco.UserInfoEndpoint);
    if(!response.IsSuccessStatusCode)
    {

    }
    else

    {
        var content = await response.Content.ReadAsStringAsync();
        IdentityResponseTextBlock.Text = content;
    }
}
```
- 请求格式

```
POST /oauth/token
grant_type=password
&Username=xxx
&Password=xxx
&client_id=xxx
&client_secret=xxx
```