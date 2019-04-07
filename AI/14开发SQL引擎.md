发现问题：比如在`Hive`中的`Hive QL`语法不像SQL语法那么全，如果可以接受SQL标准语法呢？

了解原理`Hive`的原理：

- 输入Hive QL语句交给语法转换器
- 语法转换器生成Hive抽象树语法Hive AST,交给语义分析器
- 从语义分析器出来，交给`MapReduce`执行计划，联通`Hive`执行函数交给`Hadoop`

确定目标： 替换`Hive`的语法转换器，生成Hive抽象树语法Hive AST

思想：装饰器模式和组合模式

