天时。Spark在2012年的时候，在摩尔定律的作用下，内存和性能大幅度提升，并且针对大数据的机器学习需求很强烈。

分治更细。MapReduce一个应用一次运行只有一个map和reduce，如果规模很大需要开数万个应用。而spark一开始针对大数据和机器学习，知道机器学习需要大量迭代，数万个计算阶段，所以Spark把整个计算分成很多阶段。

少做事。Spark减少了对HDFS的访问次数，减少作业调度的次数。

空间换时间。Spark很多都保存在内存里，而不像Map Reduce写在磁盘里。

Spark大概做法：程序代码交给Spark的`DAGScheduler`，转换成DAG有向无环图，然后交给分布式集群。

Spark如何划分计算阶段？RDD之间转换连接线多对多交叉连接就会产生计算阶段，一个RDD代表一个数据集，每个RDD有很多小块，叫RDD数据切片，一个数据集多个数据分片会写入到另一个数据集不同分片，这也是shuffle过程，也将数据重新组合，相同key在一起，进行聚合等操作。

Spark执行过程：在JVM中运行进程，`Spark Context`初始化配置接受输入数据，通过后`DAGScheduler`生成DAG图，交给`cluster manager`申请计算资源，然后在集群里创建Worker Node, Woker Node向`Spark Context`注册，`Spark Context`分配任务，Worker Node的Executor开始执行，基于反射加载程序。