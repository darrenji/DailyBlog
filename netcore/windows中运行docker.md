在windows运行docker有两种方式，一种是开启Hyper-V,一种是有virutal box。

大致过程：

- 下载安装docker for windows
- 启动Hyper-V,管理员打开Power Shell
```
Enable -WindowsOptionalFeature -online -FeatureName Microsoft-Yper-V -All
```
- 启动docker, 右下角有一个图标，移动上去显示docker is running,登录的话可以连接到Docker Hub
- .NET Core创建项目，启用Docker支持，根目录下多出一个Dockerfile文件，修改属性，复制到输出目录=不复制
- 确认docker是否正常，在Power Shell

```
docker info
```
- 构建docker镜像，在项目根目录下运行

```
cocker build -t demotest 
```

- 运行：cocker run  --name=demotest -p 7788 -d demotest