```
public class EamilService
{
	private ConcurrentQueue<string> _qeuue = new ConcurrentQeuue<string>();

	//不同的线程会使用这个方法
	//很多场景下，不知道什么时候会调用这里的AddEmail方法
	public void AddEmail(string email)
	{
		_queue.Enqueue(email);
	}

	public void StartSendingEmail()
	{
		While(true){
			bool isNotEmpty = _queue.TryDequeue(out string email);
			if(isNotEmpty){
				SendEmail(email);
			} else {
				Thread.Sleep(1000);
			}
		}
	}

	private void SendEmail(string email){}
}
```

使用BlockCollection。

```
public class EmailService
{
    //默认使用了ConcurrentQueue
	private BlockingCollection<string> _collection = new BlockCollection<string>();

	public void AddEamil(string email){
		_collection.Add(email);
	}

	public void StatSedingEmail(){
		while(true){
			//这里的Take,如果有就会从队列里取出队列项，如果没有就会什么都不做
			string email = _collection.Take();
			SendEmail(email);
		}
	}
}
```