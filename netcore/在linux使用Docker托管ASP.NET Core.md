Docker可以运行在linux上，或者windows虚拟机上。Docker有一个客户端。

- 创建项目：`dotnet new webapi -o ToDoAPI`
- vs code打开：`code -r TodoApi`
- 运行：`dotnet run`

> Dockerfile, 根据其中的指令构建镜像，在其中定义了单个容器的内容和启动行为。大致如下：

```
FROM microsoft/dotnet:2.2-sdk AS build-env WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY ../
RUN dotnet publis -c Release -o out
FROM microsoft/dotnet:
-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out
ENTRYPOINT ["dotnet", "TodoApi.dll"]

```

> Dockerignore 告诉Docker可以忽略的文件

- 在TodoApi的根目录中构建镜像：`docker build -t todoapi`
- 构建完镜像确认本地镜像仓库中是否存在构建的镜像：`docker images`
- 运行容器：`docker run --rm -it -p 5000:80 todoapi`

```
--rm 容器推出后自动删除容器
-it 以交互的模式运行容器，为容器分配一个伪输入终端，方便输出和调试
```

- 生产环境下运行容器：`docker run -d --restart=always --name myapp -p 5000:80 todoapi`

> 如果有多个容器内，那就需要nginx作为反向代理。

> Docker Compose，用来多容器的一键部署，通过YAML文件配置。

- 下载Docker Compose:`sudo curl -L ".."`
- 对下载的文件添加可执行程序：`sudo chmod ...`
- 测试是否安装Docker Compose成功：`docker-compose --version`
- 为API项目添加redis:`dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis --version 2.2.0`
- 在API项目中的`Startup.cs`中配置：

```
services.AddStackExchangeRedisCache(options => options.Configuration = Configuration.GetConnectionString("Redis"));
```
- 创建docker-compose.yml:其中都可以看作是服务，把服务看成生产环境下的容器

> 容器一旦删除就会出现跟它相关的数据都会丢失。Docker引入数据卷volumes机制，将宿主的某个目录或文件映射挂载到Docker容器。

- 在API项目下的appsettings.json中配置
- 在API根目录下床架目录：`mkdir conf`
- 创建redis服务，从官方下载一个配置文件，修改Security节点
- 创建nginx服务，也放在conf目录中
- 运行docker-compose:`docker-compose up -cl`
- 确认容器应用：`cocker-compose ps`

written on 2019年5月8日