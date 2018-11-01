Kestrel和SignalR可以配合使用吗？Yes。

实现这样一个工作场景：有一个客户端把数据源源不断低传给Hub,然后Hub推送给其它客户端，达到实时显示的效果。

文件结构如下：

```
源源不断提供数据的客户端：ChartExample.ConsoleApp
模型放在：ChartExample.Core
Hub所在的网站：ChartExample.Web
```

## 网站 ##


首先是Hub:

```
public class SensorHub:Hub
{
	public Task Broadcast(string sender, Measurement measurement)
	{
		return Clients
			.AllExcept(new[] {Context.ConnectionId})
			.InvokeAsync("BroadCast", sender, measurement);
	}
}
```

一切的精华都在这里。提供数据源的客户端调用Hub的Broadcast方法，而Broadcast方法调用了客户端的方法。

Hub需要暴漏给外界。在Startup中定义：

```

    public class Startup
    {
       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddCors(o => {
                o.AddPolicy("Everything", p => {
                    p.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseFileServer();
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("Everything");

            app.UseSignalR(routes => {
                routes.MapHub<SensorHub>("sensor");
            });
        }
    }
```

有关Kestrel的配置，在Prgram.cs中：

```
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseSetting(WebHostDefaults.PreventHostingStartupKey, "true")
                .ConfigureLogging(f =>
                {
                    f.AddConsole();
                })
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

      
    }
```

能保证运行的组件如下：

```
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Cors" Version="2.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics" Version="2.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.Hosting" Version="2.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.Server.Kestrel" Version="2.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.0.0-alpha2-final" />
    <PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="2.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.CommandLine" Version="2.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="2.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ChartExample.Core\ChartExample.Core.csproj" />
  </ItemGroup>

</Project>
```

## 数据模型 ##

Measurement由于需要用来传输，需要` Newtonsoft.Json`的辅助。

```
    public class Measurement
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        public override string ToString()
        {
            return $"Meaurement (Timestramp = {Timestamp}, Value = {Value})";
        }
    }
```

用到的组件：

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="10.0.3" />
  </ItemGroup>

</Project>

```

## 提供数据的客户端 ##

```
    class Program
    {
        private static readonly ILogger logger = CreateLogger("Program");

        static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => MainAsync(cancellationTokenSource.Token).GetAwaiter().GetResult(), cancellationTokenSource.Token);

            Console.WriteLine("Press Enter to Exit ...");
            Console.ReadLine();

            cancellationTokenSource.Cancel();
        }

        private static ILogger CreateLogger(string loggerName)
        {
            return new LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .CreateLogger(loggerName);
        }

        private static async Task MainAsync(CancellationToken cancellationToken)
        {
			//定义和Hub的连接
            var hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/sensor")
                .Build();

			//开始
            await hubConnection.StartAsync();

            Random rnd = new Random();
            double value = 0.0d;

            while(!cancellationToken.IsCancellationRequested)
            {
                //停顿
                await Task.Delay(250, cancellationToken);

                //产生数据，并记录日志
                value = Math.Min(Math.Max(value + (0.1 - rnd.NextDouble() / 5.0), -1), 1);
                var measurement = new Measurement() { Timestamp = DateTime.UtcNow, Value = value };
                if(logger.IsEnabled(LogLevel.Trace))
                {
                    Console.WriteLine($"Broading Measurement to Clients {measurement}");
                }

                //发送
                await hubConnection.InvokeAsync("Broadcast", "Sensor", measurement, cancellationToken);

            }

            await hubConnection.DisposeAsync();
        }
    }
```

用到的组件：

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="1.0.0-alpha2-final" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="2.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ChartExample.Core\ChartExample.Core.csproj" />
  </ItemGroup>

</Project>
```

## 客户端 ##

```

            var connection = new signalR.HubConnection("sensor");
			//客户端响应Hub的方法
            connection.on('Broadcast',
                function(sender, message) {
                    values.push(message.value);
                    values.shift();
                    chart.update();
                });
            connection.start();
```

在近期项目中，前一段时间使用了Stream来实现实时性，现在来看，也可以换种方式，可以通过或IHostedServcie定时不间断调用Hub方法。

```
public class MyBackgroudService : IHostedService
{
	private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
	private readonly IHubContext<MyHub> _hubContext;

	public MyBackgroundService(IHubContext<MyHub> hubContext)
	{
		_hubContext= hubContxt;
	}

	public Task StartAsync()
	{
		_  = BackgroundTask(_cancellatioinTokenSource.CancellationToken);
	}

	public Task StopAsync()
	{
		_cancellationTokenSource.Canel();
	}

	private Task BackgroundTask(CancellationToken cancellationToken)
	{
		while(!cancellationToken.IsCancellationRequested)
		{
			//使用_hubContext
		}
	}
}
```

> 控制器中如何调用Hub中的方法？

方式一：使用IHubContext.

```
private IHubContxt<SomeHub> HubContext{get;set;}

public SomeController(IHubContext<SomeHub> hubContext)
{
	HubContext = hubContext;
}

await this.HubContext.Clients.All.InvokeAsync("Completed",id);
```

方式二：申明一个类型的Hub.

```
public interface ITypedHubClient
  {
    Task BroadcastMessage(string name, string message);
  }

public class ChatHub : Hub<ITypedHubClient>
      {
        public void Send(string name, string message)
        {
          Clients.All.BroadcastMessage(name, message);
        }
      }

[Route("api/demo")]
  public class DemoController : Controller
  {   
    IHubContext<ChatHub, ITypedHubClient> _chatHubContext;
    public DemoController(IHubContext<ChatHub, ITypedHubClient> chatHubContext)
    {
      _chatHubContext = chatHubContext;
    }
    // GET: api/values
    [HttpGet]
    public IEnumerable<string> Get()
    {
      _chatHubContext.Clients.All.BroadcastMessage("test", "test");
      return new string[] { "value1", "value2" };
    }
  }
```