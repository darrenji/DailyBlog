AzureDevOps用来完成容器自动化部署,完成持续集成和持续部署。
地址：https://azure.microsoft.com/zh-cn/services/devops/

大致过程：

- 代码提交到reposioty
- 触发CI持续集成，在构建服务器上build app, test app, build Image
- 从Azure Container Registry中pull
- push到Azure Container Registry中
- 触发CD持续部署，从Azure Container Registry中pull


几个文件：
- Dockerfile
- azure-pipelines.docker.yml配置编译和发布
- docker-compose-template.yml
