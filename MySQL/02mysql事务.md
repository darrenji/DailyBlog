当需要对多个表进行操作会用上事务。默认情况下执行一条sql语句就会自动执行，而在执行事务的时候可以把自动执行关掉。

```
SET autocommit = 1;
SET autocommit = OFF;
```

比方两张表：

```
public class orders
{
	public int orderNumber;
	public datetime orderDate;
	public datetime requiredDate;
	public datetime shippedDate;
	public string status;
	public string comments;
	public string customerNumber;
}

public class orderdetails
{
	public int orderNumber;
	public string productCode;
	public int quantityOrdered;
	public decimal priceEach;
	public string orderLineNumber;
}
```

执行下面的事务：

```
START TRANSACTION;

SELECT 
	@orderNumber: =MAX(orderNumber) +1;
FROM
	orders;

INSERT INTO orders(orderNumber,
					orderDate,
					requiredDate,
					shippedDate,
					status,
					customerNumber)
VALUES(@orderNumber, '','','','',5);

INSERT INTO orderdetails(orderNumber, productCode, quantityOrdered, priceEach, orderLineNumber) 
VALUES(@orderNumber, '',30,'',1),(@orderNumber, '',30,'',1)

COMMIT;
```

> 当多个事务同时执行的时候，情况就复杂起来。这边在读着数据，那边在改着数据的情况时有发生。为了让不同的事务同时运行，需要指定一些规则，着就是`事务的隔离`。它遵循了ACID(Atomicity, Consistency, Isonlation, Durability)，当然现在还无法体会到这些抽象层面的特点。

有时候，当事务还没有提交的时候就可以读数据，这就是`读未提交`。

有时候，需要等到其它事务提交了才能读数据，这就是`读已提交`。

有时候，希望在执行事务的时候数据一致不变化，这就是`可重复读`。

还有，当一个事务执行的时候不希望其它事务执行，需要对其它事务加锁，这就是`串行化`。

如何查看当前数据库的事务呢？

```
show variables like 'transaction_isolation';
```

> `可重复读`在哪里能用上呢？

比如银行用户的余额和交易明细，每月初需要进行校对，在校对的时候不希望数据有变化，这时候`可重复读`就派上用场了。

> `可重复读`大体是怎么实现的？

所有的操作记录会被记录在回滚日志里，每一个节点日志都回有其对应的read view,而事务的操作就是围绕read view来进行的。当在一个事务里进行操作，实际上是在特定的read view中操作，其它事务不会受影响。

> 回滚日志什么时候删除呢？

当事务没有用到read view,这个read view相关的回滚日志就会被删除。

> 有些事务很长呢？

长事务通常由由很多的read view,占用很大的内存空间，应该避免。应该把自动提交关掉`SET autocommit=1`，然后显示地写事务。

- 使用begin, start transaction, commit, rollback
- set autocommit=1;
- commit work and chain

> 如何查找事务内？

```
select * from information_schema_innodb_trx where TIME_TO_SEC(timediff(now(),trx_started)>60);
```

> 事务是用来解决复杂操作的，当事务多的时候先指定一些规则，也就是隔离策略会让事情变得简单一些，然后在实现各个隔离策略的时候，有时候需要把状态保存下来。

> 避免常事务造成的不好影响

1、应用端：把不必要的事务去掉，避免单个语句执行太长，把自动提交关掉。

```
Set autocommit=1;
Set MAX_EXECUTION_TIME
```


2、数据库端：设置事务的阙值，设置回滚的粒度。

```
infomation_schema.Innodb_trx表
innodb_udo_tablespaces 
```