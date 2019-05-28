- 下载安装visual studio
- 准备visual studio 
- 下载安装windows下的git：http://git-for-windows.github.io/
- 查看sdk:dotnet --list-sdks
- 查看运行时：dotnet --list-runtimes
- Gitea创建新账户：
- 来到管理员本地代码库：git status
- 查看分支：git branch
- 查看历史：git log --oneline -n10
- 基于master上最新的commit创建并切换分支：git checkout -b development a8c8eb3
- 上传新的分支：git push origin development
- 在新的分支下创建新的内容
- 查看和master有没有冲突：git checkout master,  git pull origin master, git checkout development
- git add --all
- git commit -m "development branch test"
- git rebase master
- git push origin development, 这样远端的development有了更新
- 在远端的development分支上，创建合并请求，于是在master分支上有了刚才在development分支上的内容
- 在远端仓库设置里禁止合并
- 来到本地仓库：git checkout master
- git pull orgin master
- git log --oneline
- git checkout development,修改文件
- git add --all
- git commit -m "development branch second change"
- git rebase master
- git push origin development
- git push origin development:master, 这样就合并到master分支上了，管理员才可以，估计普通用户不可以，因为远端已经禁用合并了
- 为项目增加一个普通用户，权限为可写

## 普通用户操作

- 安装完visual studio 
- 安装完git bash
- 安装完sdk
- 安装完runtime
- 检查版本：git --version
- dotnet --list-sdks
- dotnet --list-runtimes
- 来到源代码服务器地址，登录，看到了项目
- 打开命令行终端：ssh-keygen -t rsa -C "qdjjx9441@sina.com"
- ssh默认保存位置：C:\Users\Administrator\.ssh\id_rsa.pub
- 拷贝id_rsa.pub中的内容到设置，管理SSH密钥，增加密钥
- git clone -b development ....git,把某个分支拷贝下来
- 修改
- git status
- git config --global user.email "qddjjx9441@sina.com"
- git config --global user.name "qdjjx"
- git commit -m "mofify from normal user the first time"
- git push origin development, 在远端的development分支上看到了刚才的提交
- git push origin development:master, 也可以合并，怎么禁用合并呢？



## 管理员用户操作

- git branch,发现在development分支上
- git pull origin development
- git checkout master
- git pull origin master