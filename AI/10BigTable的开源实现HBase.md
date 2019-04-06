Google在2004年发表了三篇论文，包括GFS, MapReduce, BigTable，GFS的开源实现是HDFS， MapReduce的开源实现是Hadoop MapReduce, BigTable的开源实现是HBase。

HBase用来解决海量数据的存储与访问，是NoSQL, 非关系数据库，分布式。

关系数据库的问题在于数据约束太多，而且有时包含了业务逻辑。

而非关系数据库的基本思想是：数据库就是用来存储数据，不应该有关系或业务逻辑，数据库就是要保持简单粗暴。

HBase的基本组成：

- HMaster: 用来管理所有的HRegion，包括HRegion的key,HRegion所在HRegionServer地址和访问端口号
- HRegion: 数据的存储单位，位于HRegionServer上
- Zookepper: 管理HMaster,通过Zookeeper选择一个HMaster


HBase的工作过程大致：

- 应用程序： 向ZooKeeper请求HMaster地址
- 应用程序：向HMaster输入HRegionServer的地址作为key
- 应用程序：访问HRegionServer
- HRegionServer: 访问HRegion实例，如果HRegon空间不够就分裂出两个， HReginServer和HMaster之间保持通信

HBase特色：
- 可扩展数据结构： ColumnFamily, 列族， 数据写入时再指定key-value
- 高性能：LSM树数据结构，Log Struct Merge Tree, Log结构合并树， Log方式连续写入，异步对数据库磁盘多个LSM树进行合并，数据的插入修改删除都在内存中进行，数据量超过设定内存阈值，就将内存排序树和磁盘上最新的排序树合并，读的时候先从内存读取，如果没有再从磁盘寻找




