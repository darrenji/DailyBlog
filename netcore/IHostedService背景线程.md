别看`IHostedService`只有两个简单的接口，它可以很强悍地执行背景线程。

```
public interface IHostedService
{
	Task StartAsync(CancellationToken cancellationToken);
	Task StopAsync(CancellationToken cancellationToken);
}
```

可以结合定时器每隔一段时间来做一些事。

```
//注意实现IHostedService的类必须自己进行垃圾回收
public class SomeBackgroundClass:IHostedService, IDisposable
{
	private readonly ILogger _looger;
	private Timer _timer;
	public IServiceProvider Services{get;}

	public SomeBackgroundClass(IServiceProvider services, ILogger<SomeBackgroundClass> logger)
	{
		Services = services;
		_logger = logger;
	}

	public void Dispose()
	{
		_timer?.Dispose();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes);
		return Task.CompltedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_timer?.Change(Timeout.Infinite,0);
		return Task.CompletedTask;
	}

	private void DoWork(object state)
	{
		using(var scope = Services.CreateScope())
		{
			usiing(var db = scope.ServiceProvider.GetRequiredService<Context>())
			{}
		}
	}
}
```

以上，要执行的动作是写死的。如果想让执行的动作灵活一点呢？来一个抽象类和抽象方法。

```
public abstract class BackgroundServiceOfficial : IHostedService, IDisposable
{
	//执行动作的线程
	private Task _executingTask;

	//为准备执行动作的线程而准备
	private radonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

	//让子类来继承定义
	protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

	public void Dispose()
	{
		_stoppingCts.Cancel();
	}

	//抽象类中的虚方法
	public virtual Task StartAsync(CancellationToken cancellationToken)
	{
		_executingTask = ExecuteAsync(_stoppingCts.Token);
		if(_executingTask.IsCanceled) return _executingTask;
		return Task.CompletedTask;
	}

	public virtual async Task StopAsync(CancellationToken cancellationToken)
	{
		if(_executingTask==null) return;

		try
		{
			_stoppingCts.Cancel();
		}
		finlally
		{
			//收尾收得好
			await Task.WhenAny(_executingTask,Task.Delay(Timeout.Infinite, cancellationToken));
		}
	}

}
```

现在可以有子类来继承`BackgroundServiceOfficial`定义一个动作。然而这里更好玩的是，定义一个动作，并且希望这个动作是放在队列里来执行的。

```
public class QueueHostedService : BackgroundServiceOfficial
{
	private readonly ILogger _logger;
	public IBackgroundTaskQueue TaskQueue{get;}

	public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
	{
		TaskQueue = taskQueue;
		_logger = loggerFactory.CreateLogger<QueuedHostedServce>();
	}

	protected async override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while(!stoppingToken.IsCancellationRequested)
		{
			var workItem = await TaskQueue.DequeueAsync(stoppingToken);
			try
			{
				await workItem(stoppingToken);
			}
			catch(Exception ex)
			{}
		}
	}
}
```

也就是子类QueueHostedService执行的自定义方法，内部是依靠`IBackgroundTaskQueue`来实现的。而`IBackgroundTaskQueue`实际上就是让线程进队列和线程出队列。

```
public interface IBackgroundTaskQueue
{
	void QueueBackgroundWorkItem(Func<CancellationToke, Task> workItem);
	Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}
```

当然接口只是定义了抽象的方法，还需要实现的。

```
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
	private ConcurrentQueue<Func<CancellationToken,Task>> _workItems = new ConcurrentQueue<Func<CancellationToken,Task>>();

	//这个是信号器，暂停键
	private SemaphoreSlim _signal = new SemaphoreSlim(0);

	public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
	{
		await _signal.WaitAsync(cancellationToken);
		_workItems.TryDequeue(our var workItem);
		return workItem;
	}	

	public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
	{
		if(workItem == null) throw new ArgumentNullException(nameof(workItem));
		_workItems.Enqueue(workItem);
		_signal.Release();
	}
}
```

好了，接下来就是使用`BackgroundTaskQueue`。

```
private IBackgroundTaskQueue Queue{get;}

using(var scope = Services.CreateScope())
{
	Queue.QueueBackgroundWorkItem(async token => {
		using(var db = scope.ServiceProvider.GetRequriedService<TaiHeContext>())
		{}
	});
}
```