> 在上一篇中，`线性代数`的本质就是描述空间活动，而`线性代数`有2个基本活动，一个是缩放scaling,它有一个缩放的标量，叫做scaler；另一个活动就是向量相加。

来看这样一个向量：

![2-1](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/21.gif)

这就是一个向量，但可以把1和2看成是标量scaler。x方向上的一个正向刻度单位定义为i,也叫i head。y方向上的一个正向客户定义为y,也叫y head。i和j也构成了一个向量，这个向量叫基向量basic vector。所以，上面的这个向量由基向量这样得来：

```
(1)i + (2)j
```

可以这样理解：所有的向量都是某个向量缩放后相加得到的。

那如何描述向量的相加呢？这叫`Linear Combinatioin线性组合`。

那为什么说是线性Linear呢？而不说成曲线组合呢？

想象一下，两个向量相加，保持一个向量不懂，另外一个向量的标量scaler不断变化，两者相加得到的新向量的终点可以连成一条线，所以就是线性了。

如果两个向量同时缩放并相加呢？这样就可以得到一个平面上所有`Linear combination`的组合，而这些组合就叫做`span`或者`张成空间`。

好了，现在描述了向量的缩放并相加得到`Linear Combination`，而这些`Linear Combination`的集合就成了`span`或者`张成空间`，想象一下在一个平面上由了很多的带箭头的线，这样看起来很难受，所以在实际使用中，只把向量的终点显示出来，就把线段省略掉了，当看到一个点，我们自己要知道，这就代表这个一个向量，只不过把原点到终点之间的线段和箭头省略了。

上面说的是2个向量的相加，如果是3个向量相加呢？2个向量相加是在一个平面内的空间，如果第三个向量加上，出现了三维空间，那么第三个向量被称作`Lineary independent`，如果第三个向量加上后还是在一个平面内，即还是一个二位空间，那第三个向量被称作`Lineary dependent`。

最后就是一句描述向量空间的话来结尾：

> The basis of a vector pace is a set of lineary independent vectors that span full space.