在azure上装一个linux虚拟机，选择用户名密码的方式。

- 在入站规则里开放22端口(支持ssh端口)和3389端口(支持xrdp端口)
- 使用Putty或者Git Bash
- 在Putty中输入ip和默认端口22， 或者在Git Bash中：`ssh your_username@ip`
- 确保liunx处于最新状态：sudo apt-get update
- 安装远程精简版远程桌面环境xfce4：sudo apt-get install xfce4
- 安装xrdp与xfce4配合使用：sudo apt-get install xrdp
- 启用xrdp: sudo systemctl enable xrdp
- 告诉xrdp启动时使用xfce4桌面环境：echo xfce4-session > ~/.xsession
- 重启xrdp: sudo service xrdp restart
- windows远程连接
- Sesson:Xorg, 输入用户名和密码

欢迎来到Linux世界！