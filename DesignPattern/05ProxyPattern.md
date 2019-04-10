代理模式为某个类的某个接口方法添加更多的逻辑，或者说丰富某个类的某个接口方法。

首先接口。
```
public interface ISubject
{
    void Request();
}
```

某个类实现这个接口。
```
public RealSubject : ISubject
{
    public void Request()
    {

    }
}
```

现在需要一个代理，既实现接口，同时也依赖上面的`RealSubject`，进一步丰富`RealSubject`的接口方法`Request`。
```
public class Proxy : ISubject
{
    private RealSubject _realSubject;

    public Proxy(RealSubject realSubject)
    {
        this._realSubject = realSubject;
    }

    public void Request()
    {
        if(this.CheckAccess()) //加入判断是否需要执行
        {
            this._realSubject = new RealSubject();
            this._realSubject.Request();

            //增加业务逻辑
            this.LogAccess();
        }
    }

    public bool CheckAccess()
    {

    }

    public void LogAccess();
}
```