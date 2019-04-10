先来具体的。
```
public class Subsystem1
{
    public string operation1()
    {
        return "";
    }

    public string operaitonN()
    {
        return "";
    }
}

public class Subsystem2
{
    public string operation1(){}
    public string operationZ(){}
}
```

Facade简化了细节，对外提供统一的方法。
```
public class MyFacade
{
    protected Subsystem1 _sub1;
    protected Subsystem2 _sub2;

    public MyFacade(Subsystem1 sub1, Subsystem2 sub2)
    {
        _sub1 = sub1;
        _sub2 = sub2;
    }

    public string Operation()
    {
        string result = string.Empty;
        result += _sub1.operation1();
        result += _sub2.operation2();
        return result;
    }
}
```