让不兼容的两个接口对接起来可以协同工作。当有了一个第三方的接口或者类，这个类或者接口无法修改，怎样在第三方基础上加上自己的逻辑协同工作呢？

第三方是这样：

```
public class PersonRepository
{
    public void AddPerson(Person person)
    {

    }
}

public class Person
{
    public string FullName{get;set;}

    public Person(string fullName)
    {
        this.FullName = fullName;
    }
}
```

比如我们自己的类是这样：
```
public class OurPerson
{
    public string FirstName{get;set;}
    public string LastName{get;set;}

    public OurPerson(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
```
一种方式是在第三方加入我们自己的逻辑。
```
public class Person
{
    public string FullName{get;set;}

    public Person(string fullName)
    {
        this.FullName = fullName;
    }

    public Person(OurPerson ourPerson)
    {
        this.FullName = $"{ourPerson.FirstName} {ourPerson.LastName}";
    }
}
```

以上有两个问题：

- Person依赖OurPerson
- 不一定可以修改Person

还有一种方式在原来类的基础上包裹一层。
```
public class OurPerson : Person
{
    public string FirstName{get;set;}
    public string LastName{get;set;}

    public OurPerson(string firstName, string lastName) : base($"{firstName}, {lastName}")
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
```

以上同样OurPerson依赖了Person。

而通常希望Person和OurPerson之间尽量减少依赖。

```
public class PersonAdapter
{
    public Person ConvertToPerson(OurPerson person)
    {
        return new Person($"{person.FirstName} {person.LastName}");
    }
}
```

于是就这样使用：

```
var ourPerson = new OurPerson("","");
var adapter = new PersonAdapter();
var person = adapter.ConvertToPerson(ourPerson);
```

还可以写一个扩展方法：
```
public static class OurPersonExtensions
{
    public static Person ToPerson(this OurPerson person)
    {
        return new Person($"{person.FirstName} {person.LastName}");
    }
}
```