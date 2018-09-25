并行编程会用到多线程，而多线程需要让集合是线程安全的。每一种集合都有对应的线程安全的集合，比如`Dictionary`对应着`ConcurrentDictionary`。

## ConcurrentDictionary ##

```
namespace somenamespace
{
	internal static class Program
	{
		private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();

		public static void AddParis()
		{
			bool success = capitals.TryAdd("France","Paris");
			string who = Task.CurrentId.HasValue ? ("Task " + Task.CurrentId) : "Main Thread";
			Console.WriteLine($"{who} {(success ? "added" : "did not added")} the element");
		}

		static void Main()
		{
			Task.Factory.StartNew(AddParis).Wait();
			AddParis();

			//在主线程里加数据
			capitals["Russia"] = "Leningrad"; //第一次是存入值
			capitals["Russia"] = "Moscow"; //第二次是修改值

			Console.WriteLine(captials["Russia"]);
		}
	}
}
```

以上的这两行代码：

```
capitals["Russia"] = "Leningrad"; //第一次是存入值
capitals["Russia"] = "Moscow"; //第二次是修改值
```

对于添加或修改，还有一个方法来实现，那就是`AddOrUpdate`，而且这个方法对整个过程有更细节的控制。

```
capitals["Russia"] = "Leningrad";
capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + " --> Moscow");

//result: The capital of Russia is Leningrad --> Moscow
//也就是把原来的值和新的值结合起来，通过func生成了一个新的值
Console.WriteLine($"The capical of Russia now is {capitals["Russia"]}");
```

那如果把上面的代码移掉一行呢？

```
capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + " --> Moscow");
//The capital is Moscow
//很显然，只有原先字典里已经有值再更新的时候，才执行AddOrUpdate中的委托才起作用
Console.WriteLine($"The capical of Russia now is {capitals["Russia"]}");
```

如果把查找和添加放到一起的话，那就用`GetOrAdd`方法。

```
capitals["Swiden"] = ""uppsala;
var capOfSweden = capitals.GetOrAdd("Sweden", "Stockholm");
Console.WrtieLine($"The capital of Swden is {capOfSweden}");
```

接下来就是移除。

```
const string toRemove = "Russia";
string removed;
var didRemove = capitals.TryRemove(toRemove, out removed);
if(didRemove)
{
	Console.WriteLine($"we just removed {removed}");
}
else
{
	Console.WriteLine($"Failed to remove the capital of {toRemove}");
}
```

如果需要知道集合中的数量，就用`capitals.Count`,但是这里需要注意的是：因为是多线程，统计数量是比较昂贵的操作，并不像在单线程中这么轻而易举，需要尽量避免使用类似的操作。

最后就是遍历了。

```
foreach(var kv in capitals)
{
	Console.WriteLine($"-- {kv.Value} is the capital of {kv.Key}");
}
```