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

## ConcurrentQueue ##

先进先出。

```
var q = new ConcurrentQueue<int>();
q.Enqueue(1);
q.Enqueue(2);

int result;
if(q.TryDequeue(out result))
{
	// 1
	Console.Writeline($"remvoed element {result}");
}
```

如果想得到队列中的最后一个。

```
if(q.TryPeek(out result))
{
	// 2
	Console.WriteLine($"front element is {result}");
}
```

## BlockingCollection ##

从一个线程把元素放到集合中，从另外的线程把元素取出来。ConcurrentBag,ConcurrentQuee, ConcturentStatck都是实现了同样的接口，所以方法都大致雷同。都实现了`IProducerConsumerCollection<T>`接口，这是一个生产消费的接口。比如从网络上获得数据放到ConcurrentBag中，然后让一个线程从ConcurrentBag中拿数据。在这里生产消费体现了出来，一方面一个Producer生产数据放到ConcurrentBag中，另一方面另外的线程一个一个来consume这个ConcurrentBag中的内容。这里有一个问题需要注意：当Consume的时候，不希望Producer产生新的数据，这时候就会用到ConcurrentBag的一个包裹类，Wrapper，那就是`BlockingCollection`。

```
//这里的10代表着ConcurrentBag这个线程安全的集合最多放10个元素进来
static BlockingCollection<int> messages = new BlockingCollectioin<int>(new ConcurrentBag<int>(),10);
static CancellationTokenSource cts = new CancellationTokenSource();
static Random random = new Random();

static void Main(string[] args)
{
	Task.Factory.StartNew(ProduceAndConsume, cts.Token);
	Console.Readkey();
	cts.Cancel();
}

static void ProduceAndConsume() 
{
	var producer = Task.Factory.StartNew(RunProducer);
	var consumer = Task.Factory.StartNew(RunConsumer);

	try
	{
		Task.WaitAll(new[] {producer, consumer},cts.Token);
	}
	catch(AggregationException ae)
	{
		ae.Handle(e =>true);
	}
}
private static void RunProducer()
{
	while(true)
	{
		cts.Token.ThrowIfCancellationRequested();
		int i = random.Next(100);
		message.Add(i);
		Console.WriteLine($"+{i}\t");
		Thread.Sleep(random.Next(1000));
	}
}

private static void RunConsumer()
{
	//从BlockingCollection中获取元素用GetConsumingEnumerable方法，这个挺特别
	foreach(var item in messages.GetConsumingEnumerable())
	{
		cts.Token.ThrowIfCancellationRequested();
		Console.WriteLine($"-{item}\t");
		Thread.Sleep(random.Next(1000));
	}
}
```

## ConcurrentBag ##

stack先进后出，queue先进先出，concurrentbag是无序的。

```
var bag = new ConcurrentBag<int>();

//一下开出10个线程，这种方式比较受用
var tasks = new List<Task>();

for(int i=0; i<10;i++)
{
	var i1=i;

	//开一个线程，放到线程集合中去
  	tasks.Add(Task.Factory.StartNew(()=> {
		bag.Add(i1);
		Console.WriteLine($"{Task.CurrentId} has added {i1}");
		int result;
		if(bag.TryPeek(out result))
		{
			Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
		}
  	}));
}

Task.WaitAll(tasks.ToArray());

int random;
if(bag.TryTake(out last))
{
	//这里得到的数据是随意的，这点也可以看出CurrentBag是无序的
	Console.WrtieLine($"I got {last}");
}
```

## 的 ##

```
const int count = 50;
var items = Enuemrable.Range(1, count).ToArray();
var results = new int[count];

//x就是集合中的元素,x是从1开始的
items.AsParallel().ForAll(x => {
	int newValue = x*x;

	//这里完全是无序的
	Console.Write($"{newValue}(){Task.CurrentId}\t");

	results[x-1] = newValue;

	Console.WriteLine();
	Console.WriteLine();

	foreach(var i in results)
	{
		Console.WrieLine($"{i}\t");
	}
	Cosole.WriteLine();
});
```

以上把集合转换成Parallel，然后获取到的集合是无序的。

转换成Parallel后，如果想按顺序处理呢？

```
const int count =50;
var items = Enumerable.Range(1,count).ToArray();

var cubes = items.AsParallel().AsOrdered().Select(x => x*x);
foreach(var i in cubes)
{
	Console.WriteLine($"{i}\t");
}
Console.WriteLine();
```

> 以上是我在当前项目中需要解决的一个问题，把数量比较大的集合转换成Parallel处理。如果不需要返回结果，就AsParallel然后ForAll；如果需要返回结果就AsParallel，然后Select,当然还可以排序，比如AsOrdered。

> 还需要总结的是，ConcurrentDictionary, Producer-consumer Collections包括ConcurrentQueye, ConcurrentStack, ConcurrentBag，Producer-consumer patter用到了BlockingCollection.

## ConcurrentStack ##

先进后出。

```
var stack = new ConcurrentStack<int>();
stack.Push(1);
stack.Push(2);
stack.Push(3);
stack.Push(4);

int result;
if(stack.TryPeek(out result))
{
	//2
	Console.WriteLine($"{result} is on top");
}

if(stack.TrypPop(out result))
{
	if(stack.TryPop(out result))
	{
		//2
		Console.WriteLine($"popped {result}");
	}
}

//如果想pop多个呢？
var items = new int[5];
if(stack.TryPopRange(items, 0,5)>0)
{
	var text = string.Join(", ", items.Select(i => i.ToString()));
	Console.WriteLine($"Popped these items: {text}");
}
```

## Parallel Linq Cancellation and Exceptions ##

```
var cts = new CancellationTokenSource();

var items = ParallelEnumerable.Range(1, 20);

//注意这里加了允许取消
var results = items.WithCancellation(cts.Token).Select(i => {
	double result = Math.Log10(i);
	
	if(result >1) throw new InvalidOperatioinException();

	Console.WriteLine($"i={i},tid={Task.CurrentId}");
	return result;
});

try
{
	foreach(var c in results)
	{
		if(c>1)
		{
			cts.Cancel();
		}

		Console.WriteLine($"result = {c}");
	}
}
catch(AggregateException ae)
{
	ae.Handle(e => {
		Console.WriteLine($"{e.GetType().Name}:{e.Message}");
		return true;
	});
}
catch(OperateCancelledException e){
	Console.WriteLine("Cancelled");
}
```

## Parallel Linq Custom Aggregation ##

使用`Sum`方法计算。

```
var sum = Enumerable.Range(1,1000).Sum();
Console.WriteLine($"sum = {sum}");
```

使用`Aggregate`计算。

```
var sum = Enumerable.Range(1,1000)
	//0是初始值，i表示当前元素，acc表示累计结果
	.Aggregate(0, (i, acc) => i + acc);
```

使用并行计算。

```
var sum = ParallelEnumerable.Range(1,1000)
	.Aggregate(
		0,
		(partialSum, i) => paritalSum += i,
		(total, subtotal) => total += subtotal,
		i => i
	);
```

## Parallel LINQ Merge ##

```
var numbers = Enumerable.Rage(1,20).ToArray();
var reuslts = numbers.AsParallel()
	.Select(x => {
		var result = Math.Log10(x);
		Console.WriteLine($"produced {result}");
		return result;
	});

foreach(var result in results)
{
	Console.WriteLine($"Consumed {result}");
}
```

注意到一点，在Parallel方式执行的时候，一批一批执行的，如果想控制如何一批一批执行呢？

```
var numbers = Enuemrable.Range(1,20).ToArray();
var results = numbers
	.AsParallel()
	//ParallelMegeOptions.FullyBuffered
	.WithMergeOptions(ParallelMergeOptions.NotBuffered)
	.Select(x => {
		var result = Math.Log10(x);
		Console.Write($"p {result}\t");
		return result;
	})

foreach(var result in results)
{
	Console.Write($"{result}\t");
}
```

> LINQ is an awsome technology for quering data, uses a number of operators, Select(), Sum(),By default exectuion is sequential, PLINQ is TPL'S counterpart for parallel LINQ.包括：AsParallel(), Cancellation and exceptions, MergeOptions,Custom Aggregation.

> turn a LINQ query paralle by AsParallel(), PrallelEnumerable;use WithCancellation() to provide a cancelllation token, AggregationException, OperationCaneledException, WithMergeOptions(ParalleMegeOptons.xxx) dertermins how soon produced results can be consumed.

## Parallel Loops breaking cancellations and exception ##

```
public static void Demo()
{
	Parallel.For(0, 20, x => {
		Console.WriteLine($"{x} [{Task.CurrentId}]\t");
	});

	static void Main(string[] args)
	{
		Demo();
	}
}
```

如何加一些控制呢？

```
Parallel.For(0, 20, (int x, ParallelLoopState state) => {
	Console.WriteLine($"{x}[{Task.CurrentId}]\t");
	if(x == 10){
		//不是立即停止，而是等其它线程执行完了再来执行
		state.Stop();
		state.Break();
	}
});
```

如何捕获异常。

```
Parallel.For(0, 20, (int x, ParallelLoopState state) => {
	Console.WriteLine($"{x}[{Task.CurrentId}]\t");
	if(x == 10){
		throw new Exception();
	}
});

static void Main(string[] args)
{
	try
	{
		Demo();
	}
	catch(AggregateException ae)
	{
		ae.Handle(e => {
			Console.WriteLine(e.Message);
			return true;
		});
	}
}
```

如何得到并行运算是否完成呢？

```
private static ParalleLoopResult result;

public static void Demo()
{
	result = Parallel.For(0, 20, (int x, ParalleLoopSate state) => {
		Console.WriteLine($"{x}[{Task.CurrentId}]\t");
		if(x==10){
			state.Break();
		}
	});

	Console.WriteLine();
	Console.WriteLine($"was loop complted? {result.IsCompleted}");
	if(result.LowestBreakInteration.HasValue)
		Console.WriteLine($"Lowes break iteration is {result.LowestBreakInteration}");
}
```

如何并行中断呢？

```
private static ParallelLoopResult result;

public static void Demo()
{
	var cts = new CancellationTokenSource();

	ParallelOptions po = new ParalleOptions();
	op.CancellationToken = cts.Token;
	result = Paralle.For(0,20, po, (int x, ParallelLoopState state) => {
		Console.WriteLine($"{x}[{Task.CurrentId}]\t");

		if(x==10)
		{
			cts.Cancel();
		}
	})
}

static void Main(string[] args)
{
	try
	{
		Demo();
	}
	catch(AggregateException ae)
	{
		ae.Handle(e => {
			Console.WriteLine();
			return true;
		});
	}
	catch(OperationCanceledException ex)
	{}
}
```
> Paralle Invoke/For/ForEach, Invoke, For, ForEach, Stopping, cancellation and exception,Thread local storage, partitioning

## Parallel Invoke for ForEach ##

以前这么写：

```
static void Main(string[] args)
{
	int[] values = new int[100];
	for(int i=0; i<100;i++)
	{
		
	}
}

```

现在这么写：

```
var a = new Action(()=> Console.WriteLine($"First {Task.CurrentId}"));
var b = new Action(()=> Console.WriteLine($"Second {Task.CurrentId}"));
var c = new Action(()=> Console.WriteLine($"Third {Task.CurrentId}"));

Parallel.Invoke(a, b, c);
```


ParallelFor和ParalleForEach

```
Parallel.For(1, 11, i => {
	Console.WriteLine($"{i*i}\t");
})

string[] words = {"",""};
Paralle.ForEach(words, word=> {
	Console.WriteLine($"{word} has length {word.Length}(task {Task.CurrentId})");
})
```

以前这么写步进：

```
public static IEnuerable<int> Range(int start, int end, int step)
{
	for(int i = start; i < end; i += step)
	{
		yield return i;
	}
}
```

现在可以这么写：


```
var po = new ParalleOptions();
po.
Parallel.ForEach(Range(1,20,3), Console.WriteLine);
```

## Parallel Loops Partition ##

安装`BenchmarkDotNet`

```
[Benchmark]
public void SquareEachValue()
{
	const int count = 100000;
	var values = Enumerable.Range(0, count);
	var results = new int[count];

	Paralle.ForEach(values, x => {
		results[x] = (int)Math.Pow(x,2);
	});
}

static void Main(string[] args)
{
	//Program要设置为Public
	var summary = BenchmarkRunner.Run<Program>();
	Console.WriteLine(summary);
}
```

使用partion。

```
[Benchmark]
public void SquareEachVlaueChunked()
{
	const int count = 100000;
	var values = Enumerable.Range(0, count);
	var results = new int[count];

	var part = Partitioner.Create(0, count, 10000);
	Paralle.Foreach(part, range => {
		for(int i = range.Item1; i < range.Items;i++)
		{
			results[i] = (int)Math.Pow(i,2);
		}
	})
}
```

## Parallel loops Thread local storage ##

如果多个线程计算得到结果。

```
static void Main(string[] args)
{
	int sum=0;
	Paralle.For(1, 1001, x => {
		Interlocked.Add(ref sum, x);
	});
}

```
以上可能不是最高效的。如果想给每个线程一个变量呢？

```
static void Main(string[] args)
{
	int sum = 0;

	Parallel.For(1, 1001, 
		() => 0,
		(x, stat, tls) => { //这里的tls就是专属于某个线程的线程变量
			tls += x;
			return tls;
		},
		partialSum => {
			Interlocked.Add(ref sum, partialSum);
		}
	);
}
```

## Task Coordination Barrier ##

按一个阶段一个阶段地执行。

```
static Barrier barrier = new Barrier(2, b => {
	Console.WriteLine($"Phase {b.CurrentPhaseNumber} is finished");
}); 

public static void Water()
{
	Console.WriteLine("putting the kettle on takes a bit longer");
	Thread.Sleep(2000);
	barrier.SignalAndWait(); //2
	Console.WriteLine("pouring water into cup"); //0
	barrier.SignalAndWait(); //1
	Console.WriteLine("putting the kettle away");
}

public static void Cup()
{
	Console.WriteLine("finding the nicest cup of tea fast");
	barrier.SignalAndWait(); //1
	Console.WriteLine("adding tea"); 
	barrier.SignalAndWait(); //2
	Consolw.WriteLine("adding sugar");
}

static void Main(string[] args)
{
	var water = Task.Factory.StartNew(water);
	var cup = Task.Factory.StartNew(Cup);

	var tea = Task.Factory.ContinueWhenAll(new[] {water, cup}, tasks =>{
		Console.WriteLine("Enjoy your cup of tea.");
	});

	tea.Wait();
}
```

## Child Task ##

```
static void Main(string[] args)
{
	var parent = new Task(()=>{

		//detached
		var child = new Task(()=> {
			Console.WriteLine("Child task starting");
			Thread.Sleep(3000);
			Console.WriteLine("Child task finishing");
		}, TaskCreationOptons.AttachedToParent);

		//child task completed trigger handler
		var completionHandler = child.ContinueWith(t => {
			Console.WriteLine($"task {t.Id}'s state is {t.Status}");
		}, TaskContinuousOptions.AttachedToParent | TaskContinuationOptioins.OnlyOnRanToCompletion);

		//失败的情况
		var failHandler = child.ContinueWith(t => {
			Console.WriteLine("task");
		}, TaskContinuousOptions.AttachedToParent | TaskContinuationOptioins.OnlyOnFaulted);

		child.Start();
	});

	parent.Start();

	try
	{
		parent.Wait();
	}
	catch(AggregateException ae){
		ae.Handle(e => true);
	}
}
```

## Task Coordination Continuations ##

```
var task = Task.Factory.Startnew(() => {
	Console.WriteLine("Boiling water");
});

var task2 = task.ContinueWith(t => {
	Console.Writeline($"completed task {t.Id}, pour water int cup.");
});

task2.Wait();
```

具有返回结果的情况。

```
var task = Task.Factory.StartNew(() => "Task1");
var task1 = Task.Factory.StartNew(() => "Task2");

var task2 = Task.Factory.ContinueWhenAll(new[] {task, task2}, tasks => {
	Console.WriteLine("tasks completed");
	foreach(var t in tasks){
		Console.WriteLine(" -" + t.Result);
	}
	Console.WriteLine("all task done");
});

task2.Wait();
```

## Coordination Countdown Event ##

```
private static int taskCount=5;
static CountdownEvent cte = new CountdownEvent(taskCount);

static void Main(string[] args)
{
	for(int i=0;i<taskCount;i++){
		Task.Factory.StartNew(()=>{
			Console.WriteLine($"entering task {Task.CurrentId}");
			Thread.Sleep(random.Next(3000));
			cte.Signal();
			Console.WriteLine($"Ecisting taks {Task.CurrentId}");
		});
	}

	var finalTask = Task.Factory.StartNew(() => {
		Console.WrtieLine("final");
		cte.Wait(); //countdown reaches zero block
		Console.WriteLine("completed");
	});
}
```

## ManualResetEventSlim and AutoResetEven ##

```
var evt = new ManualResetEventSlim();
Task.Factory.StartNew(()=>{
	Console.Writeline("boiling water");
	evt.Set();
});

var makeTea = Task.Factory.StartNew(() => {
	Console.WriteLine("waiting ro water");
	evt.Wait(); //等待set完成 
	Console.WriteLine("here is your tea");
});

makeTea.Wait();
```

自动。

```
var evt = new AutoResetEvent(false);

Task.Factory.StartNew(()=> {
	Console.WriteLine("boiling water");
	evt.Set(); //true
});

var makeTea = Task.Factory.StartNew(() => {
	Console.WriteLine("waiting for water...");
	evt.WaitOne(); //false
	Console.WriteLine("here is your tea");
    var ok=evt.WaitOne(1000);
	if(ok){
		Console.WritLine("enjoy your tea");
	}
    else
	{
		Console.WriteLine("no tea for you");
	}
	
});

makeTea.Wait();
```

## Coordination SemaphoreSlim ##

Countdown只能倒退，但这个可以倒退也可以前进。

```
var semaphore = new SemaphoreSlim(2,10);
for(int i=0; i<20;i++){
	Task.Factory.StartNew(() => {
		Console.WriteLine($"Entering task {Task.CurrentId}");
		semaphore.Wait(); //Release count --
		Console.WriteLine($"Processing task {Task.CurrentId}");
	});
}

while(semaphore.CurrentCount <=2)
{
	Console.WriteLine($"Semaphore count: {semaphore.CurrentCount}");
	Console.ReadKey();
	semaphore.Release(2); //ReaseCount +=2
}
```