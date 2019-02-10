假设有一整套动作要做。

```
public class MvcLib
{
    public static Task ListenAsync(Uri address);
    public static Task<Request> ReceiveAsync();
    public static Task<Controller> CreateControllerAsync(Request request);
    public static Task<View> ExecuteControllerAsync(Controller controller);
    public static Task RenderViewAsync(View view);
}
```

应用程序需要消费这个类库。

```
class Program
{
    static async Task Main()
    {
        while(true)
        {
            Uri address = new Uri("http://0.0.0.0:8080/mvcapp");
            await MvcLib.ListenAsync(address);
            while(true)
            {
                var request = await MvcLib.ReceiveAsync();
                var controller = await MvcLib.CreateControolerAsync(request);
                var view = await MvcLib.ExecuteControllerAsync(controller);
                await MvcLib.RenderViewAsync(view);
            }
        }
    }
}
```

显然，应用程序直接消费类库是强耦合关系。为了松耦合，需要把类库的工作交给一个角色。比如说交给一个执行引擎，这个引擎会编排好工作流程。控制的反转在这里出现，就是把流程控制交给一个框架，或者说执行引擎，而不是交给应用程序。在MVC中很多命名符合惯例，这样做方便执行引擎可以找到。

IoC更像是一种设计原则，很多设计模式用到了这一原则。

**模板方法Template Method**

把可复用的工作流程、多个步骤组成的算法定义在一个类中，这些流程以虚方法呈现，并且提供供外界调用的一个公共方法，如果想实现对流程的定制，可以通过派生类来重写相应的虚方法。

```
public class MvcEngine
{
    public async Task StartAsync(Uri address)
    {
        await ListenAsync(address);
        while(true)
        {
            var request = await ReceiveAsync();
            var controller  = await CreateControllerAsdync(request);
            var view = await ExecuteControllerAsync(controller);
            await RenderViewAsync(view);
        }
    }

    protected virtual Task ListenAsync(Uri address);
    protected virtual Task<Request> ReceiveAsync();
    protected virtual Task<Controller> CreateControllerAsync(Request request);
    protected virtual Task<View> ExecuteControllerAsync(Controller controller);
    protected virtual Task RenderViewAsync(View view);
}
```

如果想控制流程

```
public class FoobarMvcEngine : MvcEngine
{
    protected override Task<View> CreateControllerAsync(Request request){}
}
```

以上，把控制交给了`MvcEngine`，而其内部遵循了模板方法原则。

**工厂方法**

还可以把流程中的方法定义成接口，或者组件，这样更灵活了。

```
public interface IWebListner
{
    Task ListenAsync(Uri address);
    Task<HttpContext> ReceiveAsync();
}

public interface IControllerActivator
{
    Task<Controller> CreateControllerAsync(HttpContext httpContext);
    Task ReleaseAsync(Controller controller);
}

public interface IControllerExecutor
{
    Task<View> ExecuteAsync(Controller controller, HttpContext httpContext);
}

public interface IViewRender
{
    Task RenderAsync(View view, HttpContext htttpContext);
}
```
以上，把各个环节抽象成接口，接下来就是把这些接口组合起来做事。

```
public class MvcEngine
{
    public async Task StartAsync(Uri address)
    {
        var listener = GetWebListener();
        var activator = GetControllerActivator();
        var executro = GetControllerExecutor();
        var render = GetVieRender();
        await listener.ListenAsync(address);
        while(true)
        {
            var httpContext = await istner.ReceiveAsync();
            var controller = await activator.CreateControllerAsync(httpContext);
            try
            {
                var view = await executor.ExecureAsync(controller, httpContext);
                await render.RendAsync(view, httpContext);
            }
            finally
            {
                await activator.ReleaseAsync(controller);
            }
        }
    }
    protected virutal IWebListener GetWebListener();
    protected virtual IControllerActivator  GetControllerActivator();
    protected virtual IControllerExecutor GetControllerExecutor();
    protected virtua IViewRender GetViewRender();
}
```

如果想对某个环节定制，只需要对接口进行实例化。

```
public class SingletonControllerActivator : IControllerActivator
{
    public Task<Controller> CreateControllerAsync(HttpContext httpContext){}

    public Task ReleaseAsync(Controller controller) => Task.CompledTask;
}
```

对定义流程的这个基类进行扩展。

```
public class FoobarMvcEngine : MvcEngine
{
    protected override ControllerActivator GetControllerActivator() => new SingletonControllerActivator();
}
```

**抽象工厂**

还可以把创建对象的事交给抽象工厂。

```
public interface IMvcEngineFactory
{
    IWebListener GetWebListener();
    IControllerActivator GetControllerActivator();
    IControllerExecutor GetControllerExecutor();
    IViewRender GetVieRender();
}

//这里是默认的工厂
public class MvcEngineFactory : IMvcEngineFactory
{
    IWebListenr GetWebListener();
    IControllerActivator GetControllerActivator();
    IControllerExecutor GetControllerExecutor();
    IViewRender GetViewRender();
}
```

执行引擎需要工厂。

```
public class MvcEngine
{
    public IMvcEngineFactory EngineFactory{get;}
    public MvcEngine(IMVCEngineFactory engineFactory = null) => EngineFactory = engineFactory ?? new MvcEngineFactory();

    public async Task StartAsync(Uri address)
    {
        var listener = EngineFactory.GetWebListener();
        var activator = EngineFactory.GetControllerActivator();
        var executor = EngineFactory.GetControllerExecutor();
        var render = EngineFactory.GetViewRender();
        await listner.ListenAsync(address);
        while(true)
        {
            var httpContext = awat listner.RecieveAsync();
            var controller = await activator.CreateControllerAsync(httpContext);
            try
            {
                var view = await executor.ExecuteAsync(controller, httpContext);
                await render.RendAsync(view, httpContext);
            }
            finally
            {
                await activator.ReleaseAsync(controller);
            }
        }
    }
}
```

默认的工厂需要一个默认的工厂实现。

```
public class FoobarEngineFactory : EngineFactory
{
    public override ControllerActivator GetControllerActivator()
    {
        return rnew SignletonControllerActivator();
    }
}
```

使用

```
var addres = new Uri("http://0.0.0.0:8080/mvcapp");
var engine = new MvcEngine(new FoobarEngineFactory());
engine.Start(address);
```

以上的执行引擎或者说框架，对流程进行控制可以轻易做到。而通常的框架，会有一个依赖注入容器，一些用到的服务，或者说服务对象，或者说服务实例放到这个容器中。在框架启动的时候，在全局把服务注册到DI容器中，服务实例的激活和调用交给框架。

**DI容器**

来模拟一个DI容器。

```
public static class MyDIExtension
{
    public static T GetService<T>(this MyDI di);
}
```

如果所有的接口都已经注册在DI容器，那在执行引擎中就可以依赖DI容器了。

```
public class MvcEngine
{
    public MyDI MyDI{get;}
    public MvcEngine(MyDI mydi) => MyDI = mydi;

    public async Task StartAsync(Uri address)
    {
        var listener = MyDI.GetService<IWebListener>();
        var activator = MyDI.GetService<IControllerActivator>();
        var executor = MyDI.GetService<IControllerExecutor>();
        var render = MyDI.GetService<IViewRender>();
        await listner.ListenAsync(address);
        while(true)
        {
            var httpContext = await listener.ReceiveAsync();
            var controller = wait activator.CreateControllerAsync(httpContext);
            try
            {
                var view = await executor.ExecureAsync(controller, httpContext);
                await render.RendAsync(view, httpContext);
            }
            finally
            {
                await activator.ReleaseAsync(controller);
            }
        }
    }
}
```

**构造器注入**

```
public class Foo
{
    public IBar Bar{get;}
    public Foo(IBar bar) => Bar = bar;
}
```

如果有多个构造函数。在其中一个构造函数上声明注入特性。

```
public class Foo
{
    public IBar Bar{get;}
    public IBaz Baz{get;}

    [Injection] 
    public Foo(IBar bar) => Bar = bar;
    public Foo(IBar bar, IBaz baz) : this(bar) 
        => Baz = baz;
}
```

**属性注入**

```
public class Foo
{
    public IBar Bar{get;set;}

    [Injection]
    public IBaz Baz{get;set;}
}
```

**方法注入**

```
public class Foo
{
    public IBar Bar{get;}

    [Injection]
    public Initialize(IBar bar) => Bar = bar;
}
```
以上，当Foo被初始化后，DI容器会自动调用`Initialize`方法对Bar属性赋值。

而在ASP.NET Core中，可以在方法中直接调用服务却不需要显式声明，是一种约定，或者说是一种惯例。

在`Startup`中可以看到：

```
public class Startup
{
    public void Configure(IApplicationBuilder app, IFoo foo, IBar bar, IBaz baz);
}
```

在中间件中可以看到：

```
public class Foobar Middleware
{
    private readonly RequestDelegate _next;

    public FoobarMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext httpContext, IFoo foo, IBar bar, IBaz baz)
}
```

**Service Locator**

如果想拿到服务，一种方式是通过惯例注入
```
public class Foo:IFoo
{
    public IBar Bar{get;}
    public IBaz Baz{get;}

    public Foo(IBar bar, IBaz baz)
    {
        Bar = bar;
        Baz = baz;
    }

    public async Task InvokeAsync()
    {
        await Bar.InvokeAsync();
        await Baz.InvokeAsync();
    }
}
```

还有一种方式是通过容器的GetService方法获得。可以理解为框架将服务推给应用程序。

```
public class Foo:IFoo
{
    public MyDIContainer MyDI{get;}
    public Foo(MyDIContainer myDI) => MyDI = myDI;
    public async Task InvokeAsync()
    {
        await MyDI.GetService<IBar>().InvokeAsync();
        await MyDI.GetService<IBaz>().InvokeAsync();
    }
}
```
以上方式不是依赖注入，是通过先拿到DI容器再拿到服务。而在这种情况下，DI容器体现了"Service locator"这种设计模式。

对于DI容器，框架的引擎在运行起来后会利用DI容器来提供当前所需的服务实例，DI容器的使用者是框架。而Service Locator模式中，使用服务器实例的是应用程序。可以理解为应用程序通过Service Locator把服务实例拉进来。

Mark Seemann把Service Locator称作为Anti-Pattern,因为在应用程序中会多了对DI容器或者Service Locator的依赖。而理想情况下，一个服务自身应该具备独立和自治特性，服务之间应该具有明确的界限。

在ASP.NET Core中，`IServiceProvider`是DI容器。