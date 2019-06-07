## google第三方登陆

- 登陆开发者控制台：console.developer.google.com
- 创建项目
- Credential
```
选择web application
Name:demo
Redirect_Uri: http://localhost:5000/signin-google
得到client_id, client_secret
```
- 认证服务器Startup.cs

```
services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
```

整个过程，先跳转到google,选择google账号，因为在google的开发者账户中设置了跳转地址，所以跳转到我们自己的认证服务器，然后在同意页面选择，最后登陆受我们自己服务器管理的web站点。

