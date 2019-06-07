> 单点登陆，登陆一种客户端，其它客户端就不需要登陆了。

来到验证服务器Config.cs

```
public static IEnumerable<ApiResource> GetApis()
{
    return new ApiResource[]{
        new ApiResource("api1", "", new List<string>{"location"}){
            ApiSecrets = {new Secret("api1 secret".Sha256())}
        },
        new ApiResource("api2", "Express API")
    }
}
```

添加新的客户端：

```
new Client {
    ClientId = "",
    ClientName= ""
    ClientSecrets = {new Secret("flask secret".Sha256())},
    Enabled = true,
    RequireConsent = false,
    AllowRemberConsent = false,
    AccessTokenType = AccessTokenType.Jwt,
    AlwaysIncludeUserClaimsInIdToken = false,
    AllowOfflineAccess=true,
    RedirectUris = {http://localhost:7002/oidc_callback},
    AllowdScopes = {
        "api1",
        "api2",
        IdentityServerConstants.StandardScopes.OpenId,
        ......
    }
}
```

比如在一个web站点：

```
public async Task<IActionResut> AccessApis()
{
    var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
    var apiClient = new HttpClient();
    apiClient.SetBearerToken(accessToken);

    //api1
    var response1 = await apiClient.GetAsync("http://localhost:5001/identity");
    if(!response1.IsSuccessStatusCode)
    {
        throw new Exception("Access APi1 failed");
    }
    ViewData["api1"] = api1Result;

    //api2
    var apiClient2 = new HttpClient();
    apiClient2.SetBearerToken(accessToken);
    var response2 = await apiClient2.GetAsync(http://localhost:5002/me);
    if(!response2.IsSuccessStatusCode)
    {
        throw new Exception("access api2 failed");
    }
    var api2Result = await response2.Content.ReadAsStringAsync();
    ViewData["api2"] = api2Result;

    return View();
}
```