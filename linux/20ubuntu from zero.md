云上Ubuntu服务器前期准备
```
apt update 
sudo apt update
apt-get update
sudo apt-get update
```

安装可视化桌面
```
apg-get install ubuntu-desktop
```

Windows远程桌面连接Ubuntu.Ubuntu支持xrdp和vnc两种远程桌面协议。vnc是默认的基于RFB协议的远程桌面程序，但不够友好。
```
sudo apt-get install xrdp
sudo apt-get install vnc4server tightvnserver
sudo apt-get install xubuntu-desktop
echo "xfce4-session" >~/.xsession
sudo service xrdp restart
通过windows远程桌面连接，连接成功，输入用户名和密码，终于看到ubuntu的界面！并且可以浏览网站！
```

安装TDengine
```
确认是否有systemd命令：whick systemd
登录网址：https://www.taosdata.com/cn/getting-started/ 
下载：下载deb包，输入邮件地址
安装位置：/root/Downloads/
打开命令行终端：alt+f2, gnome-terminal
查看当前位置：pwd
来到所在目录：cd Downloads
安装TDengine: sudo dpkg -i tdengine-1.6.2.2.deb
安装成功提示：
    TDengine is installed successfully!
    To configure TDengine: edt /etc/taos/taos.cfg
    To start TDengine: sudo systemctrl start taosd
    To access TDengine: use taos in shell
回到根目录： cd ~
来到TDengine的配置文件：cd /etc/taos/
检查taosd状态：systemctl status taosd
启动服务：systemctl start taosd
再次查看服务状态：systemctl status taosd
```

在linux上小试牛刀
```
taos
创建数据库：create database db;
查看所有数据库：show databases;
使用数据库：use db;
创建表：create table t(ts timestamp, cdata int);
插入数据：
    insert into t values ('2019-07-15 00:00:00',10);
    insert into t values ('2019-07-15 01:00:00', 20);

查询：select * from t;
```

Windows连接Ubuntu上的TDengine
```
初次连接失败： ./taos.exe -h ip
查看是否ping通：ping ip
安装最新的客户端版本
```

还是不行。熟悉vi的使用，修改配置文件。
```
三种命令模式：
    命令模式切换到插入模式：i
    从插入模式切回到命令模式：esc
    在命令模式下，按 :wq 表示保存并退出， 按 :q! 表示不保存并退出
三种插入模式：
    i 从光标当前位置开始
    a 从光标所在位置的下一个位置开始
    o 从光标的行首插入一行
命令模式下光标的移动：
    h 向左
    l 向右
    k 向上
    j 向下
    数字0 本行最开头
    G 最后一行最开头
    $ 本行行尾
    ^ 所在行行首
    w 下一个字母的开头
    e 下一个字母的结尾
    b 上一个字母的开头
命令模式下删除文字
    x 每按一次删除光标位置后面的一个字符
    6x 删除光标位置后面的6个字符
    X 每按一次删除光标位置前面的一个字符
    20X 删除光标位置前面的20个字符
    dd 删除光标所在行
```

修改taos.cfg配置文件
```
确认配置：sudo vi /etc/taos/taos.cfg  internalIP：是ifconfig中的地址
确认开启服务：systemctl stop taosd systemctl start taosd systemctl status taosd
确认服务器防火墙没在起作用：sudo ufw status
确认服务器开放6020-6040端口,并且是TCP和UDP都要
确认客户端命令正确：./taos.exe -h ip
ubuntu重启：reboot
```