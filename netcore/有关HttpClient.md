`HttpClient`可以接收`HttpMessageHandler`,它包含`HttpRequestMessage`和1`HttpResponseMessage`，支持扩展。

`HttpClient`模式使用`HttpClientHandler`,它继承`DelegatingHandler`。

`IHttpClientFactory`用来生产`HttpClient`。

```
public class MyHttpMessageHandler : DelgatingHandler
{
	private HttpResponseMessage _myResponse;
	public MyHttpMessageHandler(HttpResponseMessage responseMessage)
	{
			_myResposne = responseMessage;
	}

	//显然DelegatingHander有发送请求得到响应的方法
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellatonToken)
	{
		retturn await Task.FromResult(_myResponse);
	}
}
```

有一个有关用户的服务。

```
public class UserService
{
	private readonly IHttpClientFactory _httpFactory;
	public UserService(IHttpClientFactory httpFactory)
	{
		_httpFactory = httpFactory;
	}

	public async Task<List<Users>>(string url)
	{
		using(HttpClient httpClient = _httpFactory.CreateClient())
		{
			using(HttpResponseMessage response = await httpClient.GetAsync(url))
			{
				if(response.StatusCode == HttpStatusCode.OK)
				{
					List<Users> users = await response.Content.ReadAsync<List<User>>();
					return users;
				}
				return null;
			}
		}
	}
}

public class User
{
	public string FirstName{get;set;}
	public string LastName{get;set;}
}
```

比如用在单元测试中。

```
public class UserSerivceTests
{
	[Fact]
	public async Task WhenACorrectUrlIsProvided_ServiceShouldReturnAllListOfUsers()
	{
		//Arrange
		var users = new List<User>{};

		var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
		var url = "";

		var myHttpMessageHandler = new MyHttpMessageHandler(new HttpResponseMessage(){
			StatusCode=HttpStatusCode.OK,
			Content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json")
		});
		var myHttpClient = new HttpClient(myHttpMessageHandler);

		httpClientFactoryMock.CreateClient().Return(myHttpClient);

		//Act
		var service = new UserSerivce(httpClientFactoryMock);
		var result = await service.GetUsers(url);

		//Assert
		result
		.Should()
		.BeOfType<List<User>>()
		.And
		.HaveCount(2)
		.And
		.Contain(x => x.FirstName == "")
		.And
		.Contain(x => x.LastName == "")
	}
}
```

总结：当需要使用`HttpClient`的时候，通过它的工厂`IHttpClientFactory`得到。`IHttpClientFactory`通过构造函数注入，并且也方便单元测试。`HttpClient`的构造函数接收HttpMessageHandler, 它有具体的基类叫做`DelegatingHandler`,所谓的自定义HttpMessageHandler实际是继承`DelegatingHandler`这个基类。而HttpMessageHandler接收`HttpResponseMessage`。`HttpResponseMessage`→`DelegatingHandler`→`HttpClient`，`HttpClient`由`IHttpClientFactory`生产。