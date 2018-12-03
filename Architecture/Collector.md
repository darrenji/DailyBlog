> 微软架构

https://dotnet.microsoft.com/learn/dotnet/architecture-guides
https://docs.microsoft.com/zh-cn/learn/?wt.mc_id=MVPEventLearn

serivce fabric + ocelot + identity server4 + 52ABP 打造微服务平台

> 什么时候使用微服务

好处：灵活的变更交付、技术的灵活性、精确的扩展、云上。tightly scoped, strongly encapsulated, loosely coupled, independently deployable, independently scalable, lead times cut by as much as 75 percent,better agility, scalability

不用：如果对敏捷和可扩展没有要求，

> 词汇

Micorservices:

- Microservices:software development technique, variant of the service-oriented architecture(SOA), structures an application as a collection of loosely coupled services.
- Bounded Context: central pattern in domain-driven design.DDD deals with large models by dividing them into different Bounded Context and being explicit about theire interrelationships.
- DDD:disign software based on models of the undelying domain. a model acts as a ubiquitous languages to help communication between software developers and domain experts.
- autonmous:microservices are autonomous service, services can be changed with no or minimal impact on others,operational issues in onse service should have no impact on other servces.
- addressable:每个微服务都有一个唯一名称url,服务注册表模式
- isolated, decoupled:
- async communicaiton message broker:
- event bus:each service publishes an event whenever it update its data, other service subscrie to events.
- api gateway
- health checks
- service discovery
- cqrs: comamnd query responsibility segregation
- aggregate: a graph of objects that can be treated as a unit
- domain entitity
- domain events
- mediator

Docker Containers:

- linux containers:an operating-system-level virtualizaiton method for running multiple isolated linux systems(containers)
- docker image:镜像
- docker hub:通过docker hub把docker image送到网络上
- hyper-v containers
- Kubernates

> 数据采集

序列化
- Protocol buffers 是一种语言中立，平台无关，可扩展的序列化数据的格式，可用于通信协议，数据存储等。
- Converting Un-structured data to Structured using AVRO

组件
- Flume is a distributed, reliable, and available service for efficiently collecting, aggregating, and moving large amounts of streaming event data
- Beats is the platform for single-purpose data shippers
- data-x: DataX与Sqoop同样是大数据异构环境数据同步工具


> 数据存储

组件
- HDFS:Hadoop Distributed File System
- 数据库：HBase is called the Hadoop database because it is a NoSQL database that runs on top of Hadoop
- Redis
- Apache Kudu - Fast Analytics on Fast Data

类型
- Use Azure Blob Storage to store all kinds of files
- Azure page blobs
- 冷
- 热

文件格式
- textfile
- Avro is a data serialization system
- ORC: An Intelligent Big Data file format for Hadoop and Hive
- Apache Parquet is a columnar storage format available to any project in the Hadoop ecosystem

压缩方式
- Snappy is a compression/decompression library
- LZ4 is a lossless data compression algorithm that is focused on compression and decompression speed
- LZO compressed files
- gzip
- bzip2

其它
-工具：hadoop on sql开发工具



> 数据加工：计算平台

组件
- Apache Spark is a unified analytics engine for big data processing
- Flink 是一个开源的针对批量数据和流数据的处理引擎，已经发展为ASF 的顶级项目之一
- Hive是Hadoop一个程序接口，Hive让数据分析人员快速上手，Hive使用了类SQL的语法
- kafaka stream:Kafka is used for building real-time data pipelines and streaming apps

ETL
- Azcopy
- Kettle
- Nifi

> 数据治理：数据治理平台

- 暂无

> 数据分析：智能分析平台

OLAP分析
- ElasticSearch
- Druid是广告分析公司Metamarkets开发的一个用于大数据实时查询和分析的分布式实时处理系统，主要用于广告分析，互联网广告系统监控、度量和网络监控。
- Kylin是一个开源、分布式的OLAP分析引擎,它由eBay公司开发，并且基于Hadoop提供了SQL接口和OLAP接口，能够支持TB到PB级别的数据量
- IndexR

Adhoc:又称为即席查询，和通常OLTP系统数据库不同的是，Ad-hoc查询允许最终用户建立自定义的查询，查询的维度和方式都不是事先准备好的，因此无法像OLTP场景下，通过精心设计数据模型，创建索引或物化视图来提升查询的性能。因此，在Ad-hoc查询的场景下，主要依赖的是数据的顺序扫描。
- Presto
- Astro
- impala
- Phoenix
- spark

ML
- Mahout
- Spark Milib
- Rserve
- Microsoft RServe

DL
- TensorFlow
- CNTK
- Caffe/Caffe2
- Tourch
- Threano


> 数据可视化：可视化平台

工具
- Workflow工作流引擎，根据角色决定信息传递路由、内容
- job设计工具
- schedule调度管理工具
- visual可视化工具

组件
- Echart:ECharts，一个使用 JavaScript 实现的开源可视化库，可以流畅的运行在 PC 和移动设备上
- Kabana
- Grafana
- Saiku

> 开发

- YARN:依赖管理
- zookeeper:ZooKeeper is a centralized service for maintaining configuration information, naming, providing distributed synchronization, and providing group services.


> 数据流

数据采集--通讯--kafaka集群--实时数据中心-kafa集群--

1. flink/spark--es/druid/hbase/hdfs--web或app
2. 分布式事件中心--业务处理

元数据管理