Hive看作大数据仓库，大数据SQL引擎，用SQL语法写出MapReduce执行的程序。

还记得MapReduce的执行过程吗？一个键值对给到Map函数，<key, <1,25>>，然后就是shuffle过程，把相同key对应的value放在一起，组成一个<key,value集合>，比如<<2,25>,<1,1>>,然后给到reduce函数，最终把结果输出类似<<2,25>,2>。整个过程就像是SQL语句拼接的过程。所以Hive就是用来把SQL语句转换成MapReduce能接受的编程模型。

Hive组成是怎样的？Hive有客户端工具，比如JDBC输入SQL语句，然后把sql语句给到Hive的`Driver`，这个`Driver`一方面把一些元数据给到`MetaStore`，这些元数据包括表、字段、字段类型、HDFS文件路径等，`Driver`还把SQL语法的分析、解析、优化交给了`Compiler`，最终Hive使用一些内置函数，比如TableScanOperator, FilterOperator, FileOutputOperator生成有向无环图DAG交个MapReduce。


SQL引擎也有不少：

- Cloudera是Hadoop的商业公司，有MPP框架的SQL引擎impala,在所有的`DataNode`上部署相同的impala进程，impala进程间相互协作，反应是毫秒级
- Spark的SQL引擎叫作Spark SQL,将SQL语句转换成Spark的执行计划，比Hive快很多
- Hive也不甘落后，将Hive执行计划转换成Spark计算
- SaleForce的Phoenix引擎，在HBase上执行SQL引擎

SQL引擎的开发并不简单，像这种数据库、操作系统、编译器是计算机极客的编程梦想，因为是基础设施啊，用的人多。