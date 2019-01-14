从最近手头项目进展来看，是时候了解物联网云平台了，`Azure IoT`当然是需要了解的首选。

一些鲜活的案例包括：通过对设备水泵检测实现预测性维护。其中用到了`Azure Machine Learning`和`Azure IoT Edge`，最终实现高级预测分析、弹性模型管理；在智能家居方面，`Azure IoT Hub`把恒温器连接到云端，客户通过触屏、音控来控制加热或冷却；在工业物联网方面，把发动机数据发送给`Azure IoT Hub`,组合使用`Azure Data Factory`、`Azure HDInsights(用于数据的聚合和汇总)`、`Azure Blob Storage`等产生可以预测的维护模型，从而防止不定期延误的发生；还有公司在电梯中加入传感器，结合Hololens实现可视化远程诊断。

以下是从设备端、云端洞见、动作、边缘计算、工具这五个方面来了解`Azure IoT`。

**设备端**

物联网的设备端面临诸多挑战：设备通常部署在远程难以进入的地方，并且设备的电源和自带的处理资源有限，设备连接到网络的成本也不一定很低，在工业场景中设备可能采用定制的或者行业特有的通讯协议，设备需要供用户使用或者与外界交互这样就存在被篡改的可能......所有这些现状必须得以有效解决。


`Azure IoT`如何保证安全性呢？设备可以使用安全token, X.509证书，私钥。设备与`IoT Hub`之间的连接使用传输层安全协议(Transport Layer Security, TLS)，`Azure IoT`支持TLS 1.2， TLS1.1, TLS1.0,`Azure IoT Hub`允许为每个安全密钥定义策略，以控制对服务端API的访问。

`Azure IoT`又是如何配置设备呢？首先在云注册表中创建一个设备标识，这个标识用来唯一标识该设备，然后为每个设备配置唯一的标识凭证。注册表同时管理着`Device Twins`。也可以使用`Azure IoT Hub Device Provisioning Service`对一个或多个IoT hub进行零接触、实时的配置。尤其是设备规模扩大后，这种自动配置方式变得更有意义。

物联网设备生命周期内的五个阶段：计划、供应、配置、监控、退役。`Azure IoT`使用查询、方法、jobs来管理`Device Twin`。`Device Twin`是json文件用来存储设备的状态、元数据、配置、条件信息。在计划阶段，把元数据保存在`Device Twin`中，以方便查询；在供应阶段，安全地把设备交给IoT Hub，使用设备注册表创建灵活的标识和凭证，还可以使用job来进行批量操作，在`Device Twin`中配置属性来反映设备的能力和条件；在配置阶段，在保证安全和健康的情况下促进设备的批量修改和固件更新。通过`Device Twin`中的desired properties、方法、job进行批量配置；在监控阶段，收集设备的健康和运行状态并实时告知相关人员，运行`Device Twin`让设备报告实时的状态，通过强大的仪表盘报告发现最紧急的问题；最后，当设备出现故障，或者在更新、服务生命周期结束后，设备最终被更换或停用。

`Azure IoT`提供了品类丰富的设备，可以在"https://catalog.azureiotsolutions.com/"中查找，这些设备都具备`Azure IoT SDK`,可以与`Azure IoT`无缝衔接，在这里可以购买到`Microsoft Azure IoT`的入门套件、`Azure IoT Edge`相关设备、芯片、网关、工业平板电脑、传感器，等等。无论云端协议是AMQPS、AMQPS over Websockets、MQTT、MQTT over Websockets, 还是Https, 无论连接方式是Bluetooth、LAN、WIFI、LTE，还是3G，无论设备安全采用Managed PKI、Symmetric Key Provisioning、Firmware Update & Integrity、Secure Hardware Attestation、Secure Hardware Disablement、Authentication & Data Protection、 Identity Management,还是Device Management，无论读写接口是GPIO、I2C/SPI、COM(RS232,RS485,RS422),还是USB，无论工业协议是采用CAN bus、EtherCAT、Modbus、OPC Classic、OPC UA、PROFINET、ZigBee还是PPMP，无论操作系统是Windows IoT Core、Windows IoT Enterprice、Windows 8/10、Debian Linux、Arduino、Windows Server、Ubuntu Linux、IOS、Mbed、Yocto Linux、RTOS、Fedoral Linux、Android，还是Raspbian Linux,无论编程语言是C、C#、Java、JavaScript(Node)，还是Python，无论传感器是和GPS、Touch、LED、Light、Gas、Noise、Proximity、Temperature、Humidity、Pressure、Accelerometers、Weight、Soil Alkalinity、Vibrations、Image capture、Motion Detection,还是Chemical/compund presence......各种设备应接不暇。

**云端洞见**

当设备部署到云端后，剩下的就是收集、处理、路由、存储和分析数据了。物联网设备的测量和远程监控，有一个专用名词叫"遥测数据(Telemetry data)"。遥测数据通常是高并发的，需要以时间序列的格式被重组和重放。然后数据被路由到不同的端点(endpoints)和服务。

消息是指设备和云端的通讯，包括遥测数据。`Azure IoT Hub`支持设备到云端、云端到设备的双向消息，需要保证在间歇性连接情况下的弹性和负载高峰情况下的可靠性，需要确保在设备和云端之间进行至少进行一次消息传递。从设备到云端的消息来看，`Azure IoT Hub`允许我们定义路由规则以控制消息发送到哪里，整个过程无需编码。从远端到设备的消息来看，为了保证至少有一次消息到达设备，`Azure IoT Hub`会在每个设备的队列中持久化信息，设备确认收到消息后才把该消息从设备队列中删除，这种方式确保了在连接失败恢复正常后或设备故障修复后再次获取到来自云端的消息。

消息会被路由到不同的位置。有些消息需要及时被处理，有些消息需要用来分析变化或异常，有些消息需要被存储起来供日后分析。`Azure IoT Hub`提供了一个内置路由，它具有简单性、可靠性、扩展性。

路由条件的查询语言与`Device Twin`和`Device Job`一样。`Azure IoT Hub`根据消息属性所带着的条件来决定路由到哪里？如果消息与任何路由都不匹配，消息会被写入到内置消息/事件端点。实际上，当我们订阅了一个Azure服务后，这个服务就可以被放置到`Azure IoT Hub`中作为消息路由的端点，这些端点就是服务端点，也是消息路由的接收器。设备无法直接和端点交互，必须通过`Azure IoT Hub`的路由机制。Azure服务中可以被用来做服务端点的包括：`Azure storage containers`,`Event hubs`,`Service Bus queues`,`Service Bus topics`,同时`Azure IoT Hub`需要拥有对这些服务的写权限，如果在`Azure portal`中配置，写权限已经自动被带上了。当第一次为服务配置时，需要注意服务所能接受、承受的吞吐量。

还有一种把事件整合到Azure服务或者应用程序中的方式，就是使用`Event Grid`。`Event Grid`是一种可管理的事件路由服务，使用了订阅发布模型。消息路由和`Event Grid`的效果是等效的，但两者有区别。

**动作**

热路径是当遥控数据被放到流里，就可以对其实时观察。冷路径是遥控数据先不作处理存储起来。`Azure IoT Hub`可以把消息写到不同的endpoints中，所以可以同时处理热路径和冷路径。热路径动作是数据流动时实时发生的动作。分析热路径数据可以用`Azure Stream Analytics`和`Azure Time Series Insights`。当然也可以使用开源的`Apache Spark`。

`Azure Stream Analytics`是事件处理引擎，对流数据提供实时的分析计算，用来检测高并发数据流，从流中提取信息，设别patterns, trends, relationships。这些pattern用来触发其它动作，比如警告、自动工作流、存储等。

`Azure Time Series Insights`处理高并发，存储，索引，查询、分析，可视化，能够存储和管理万亿字节的时间序列数据来进行根本原因分析。

`Apache Spark`是针对热路径分析的开源解决方案，用来对大数据进行分析。Spark是基于内存计算的引擎，对大数据有超强的查询性能。Spark得益于内部并行的数据处理框架，可以把数据持久化到内存或者硬盘。

`Azure Functions`是serverless计算服务，针对不同的事件按需运行代码，支持触发器，一个最有用的触发器是`EventHubTrigger`。

`Azure Event Grid`让应用程序侦听并且响应某个特性的事件，它按操作付费。

`Azure Cosmos DB`实现全球弹性扩展，提供NoSQL支持和多个良好的一致性模型，99%保证低延迟。

`Azure Storage`提供可扩展的数据存储，比如文件服务，消息存储。

`Azure Data Lake`存储任意大小、形状、速度，支持所有的处理类型。用于身份、管理和安全，以简化数据管理和治理。它还与操作存储和数据仓库无缝集成，因此可以扩展当前的数据应用程序。

`Azure Machine Learning`用来准备数据，开发试验，部署模型，可以在容器其或者Spark clusters中做试验，也可以配合使用GPU虚拟机来加快执行。

`Azure HDInsight`使用开源框架处理大数据，可以看作是Hadoop组件的云端分布。

**边缘计算**

设备端的处理就是边缘计算。如果设备和云端之间的带宽是有限的，如果需要对设备进行实时操作比如紧急情况下停止机械臂，如果出于安全考虑有些设备不是一直联网，类似这些场景可以使用`Azure IoT Edge`。`IoT Edge modules`会被内置于边缘设备中，内部包含Azure服务，第三方服务或者其它代码，这样就可以在本地运行了。`The IoT Edge runtime`运行在边缘设备，用来管理`IoT Edge modules`。`Azure IoT Hub`为边缘设备提供接口来进行远程操作管理。

**工具**

- Azure IoT Hub Toolkit：IoT Hub management, Device management, Module management, Interact with Azure IoT Hub, Interact with Azure IoT Edge
- Azure IoT Edge: 管理Iot Edge解决方案
- Azure CLI: 用户管理Azure资源，跨平台命令行，可以通过`Azure Cloud Shell`在浏览器中使用。
- Azure IoT SKDs:开源的，需要安装在设备中，支持各种操作系统
- REST APIs: IoT Hub供外界调用，包括device, messaging, job services, resource provider.
- Azure Protal:用来创建配置IoT Hub,管理设备，endpoints, messaging routing, operations, and work with Azure IoT Edge.





