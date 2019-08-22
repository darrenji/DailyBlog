查看`NetwordManager`服务的状态：
```
systemctl status NetworkManager
service NetworkManager restart/start/stop
```

查看`network`服务的状态：
```
systemctl status network
```

最后在ubuntu上手动设置ip等解决：
```
192.168.8.110
255.255.255.0
192.168.8.1

192.168.8.1
```