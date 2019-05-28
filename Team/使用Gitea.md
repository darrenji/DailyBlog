在Windows Server上安装Gitea。

- 事先安装mysql数据库，创建数据库"gitea"
- 事先安装git:https://git-scm.com/downloads, 安装完后，任意位置右键，Git Bash Here, 输入`git --version`检查版本
- 地址： https://dl.gitea.io/gitea/master/ 
- gitea-master-windows-4.0-amd64.exe
- 把gitea-master-windows-4.0-amd64.exe放到gitea目录并改名：C:\gitea\gitea.exe
- 管理员身份打开命令行窗口
```

添加服务：
sc create gitea start=auto binPath=""C:\gitea\gitea.exe" web --config "C:\gitea\custom\conf\app.ini""

删除服务：
sc delete gitea
```
- 服务下有了gitea这个服务，启动
- 浏览：http://localhost:3000/
- 进入初始化设置：点击登录
- 填写设置并安装
```
数据库类型：MySQL
数据库主机：127.0.0.1:3306
用户名：gitea
数据库用户密码：
数据库名称：gitea
站点名称：Gitea:Git with a cup of tea
仓库根目录：C:\Windows\system32\config\systemprofile\gitea-repositories
LFS根目录：C:\gitea\data\lfs
以用户名运行：Administrator
SSH服务域名：主机IP地址
SSH服务端口：22
HTTP服务器端口：3000
Gitea基本URL：主机IP地址:3000
日志路径：
管理员账号设置：
```
- 通过公网访问：主机IP地址:3000
- 阿里云配置：安全，防火墙，添加规则
```
SSH TCP 22
gitea TCP 3000
gitea udp 3000
```
到这里已经可以通过公网访问gitea了，接下来就是创建代码库。

- 登录创建代码库
- touch .gitignore
- git init
- git remote add origin http://ip:3000/user_name/git_name.git 
- git add --all
- git commit -m "initial"

接下来配置ssh。账户的SSH公钥一旦设置，就拥有该账户下所有项目仓库的读写权限。

- ssh默认保存位置：C:\Users\Administrator\.ssh\id_rsa.pub
- 打开命令行终端：ssh-keygen -t rsa -C "darren@ddingsafe.com", 如果已经有了，就不需要生成了
- 拷贝id_rsa.pub中的内容到设置，管理SSH密钥，增加密钥
- 查看当前的remote: git remote -v
- 修改git remote: git remote set-url origin Administrator@47.103.61.198:darren/DDCloudAPI.git
- 查看当前的remote: git remote -v
- 在服务器的C:\gitea\custom\conf\app.ini中的[server]下添加`START_SSH_SERVER = true`，保存
- 重启gitea服务
- 推到远端repository:git push origin master
- 服务器保存位置：C:\Windows\System32\config\systemprofile\gitea-repositories\darren


