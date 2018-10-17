## chat example ##

实现`Hub`。

```
public class ChatHub : Hub
{
	public void Send(string name, string message)
	{
		Clients.All.SendAsync("broadcastMessage", name, message);
	}
}
```

客户端的broadcastMessage方法。

前端首先有一个建立连接的方法。

```
connection.start()
	.then(function(){
		//触发Hub方法的时候把参数传出去
		connection.invoke('send',name, messageInput.value);
	});
```

Hub调用客户端的方法是这样定义的。

```
//Hub方法中有几个参数，这里的方法就有几个参数
connection.on('broadcastMessage',function(name, message){

});
```
更完整一点的例子。

```
public class Chat : Hub
{
	public override async Task OnConnectedAsync()
	{
		await Clients.All.InvokeAsync("Send", $"{Context.ConnectionId} joined");
	}

	public override asyn Task OnDisconnectedAsync(Exception ex)
	{
		await Clients.All.InvokeAsync("Send", $"{Context.ConnectionId} left");
	}

	public Task Send(string message)
	{
		return Clients.All.InvokeAsync("Send",$"Context.ConnectionId:{message}");
	}

	public Task SendToGroup(string groupName, string message)
	{
		return Clients.Group(groupName).InvokeAsync("Send", $"{Context.ConnectionId}@{groupName}:{message}");
	}

	public async Task JoinGroup(string groupName)
	{
		await Groups.AddAsync(Context.ConnectionId, groupName);
		await Clients.Group(groupName).InvokeAsync("Send",$"{Context.ConnectionId} joined {groupName}");
	}

	public async Task LeaveGroup(string groupName)
	{
		await Gourps.RemoveAsync(Context.ConnectionId, groupName);
		await Clients.Group(groupName).InvokeAsync("Send",$"{Context.ConnectionId} left {groupName}");
	}

	public Task Echo(string message)
	{
		return Clients.Client(Context.ConnectionId).InvokeAsync("Send",$"{Context.ConnectionId}:{message}");
	}
}
```