- ubuntu官网下载18.04.2 LTS的iso镜像文件
- 在Hyper-V上安装ubuntu,网络适配器选择外网
- 在Hyper-V的ubutnu设置中的安全中，把"安全启动"勾选去掉
- 在Hyper-V上双击启动ubuntu
- 点击"Install Ubuntu 18.04.2 LTS"
- 这时的网络状态
  
```
电脑--vTthernet(for_ubuntu18)--外网
```
- 这时的网络配置

```
多了一个"网桥"，勾选IPv4协议
    IP：192.168.8.21
    子网掩码：255.255.255.0
    默认网关：192.168.8.1
    首选DNS服务器：192.168.8.1

WLAN
    已启用，桥接的
vEthernet(for_ubuntu18)
    自动获取，默认忘光是：192.168.8.1
```
- ctrl + alt + t
- sudo su
- clear
- cd /etc/netplan
- ls
- cd /
- cd /etc/netplan
- gedt 01-network.....yaml
```
network:
    version: 2
    renderer: NetworkManager
    ethernets:
        ens33:
            dhcp4: no
            dhcp6: no
            addresses: [192.168.8.45/24]
            gateway4: 192.168.8.1
            nameservers:
                addresses: [192.168.8.1]
```
- sudo netplan apply
- ping www.baidu.com
- sudo apt install net-tools
- ifconfig
```
inet:192.168.8.119
netmask:255.255.255.0
broadcast:192.168.8.255
```