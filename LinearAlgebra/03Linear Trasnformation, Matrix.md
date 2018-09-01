> `线性代数`可以描述空间，通过向量的相加和向量的缩放可以得到一个新的向量。任意的一个向量可以理解成基于基向量的i帽和j帽通过线性组合`Linear Combination`而得到。向量的缩放scaling由标量`scalers`引起，而向量的相加就是`Linear Combination`。当向量相加的时候，如果一个向量对原来的张成空间没有影响，维持在同一个平面内，那这个向量叫做`Linear Dependent`;如果一个向量对原来的张成空间造成了影响，出现了新的张成空间，那这个向量叫做`Linear Independent`。接下来需要操作空间，对于空间内的两个向量，怎样从一个向量转换成另外一个向量，如何操作呢？如何描述这个过程呢？

当把一个向量转换成另外一个向量，从空间的角度来看，会有无数种方式，而且有些变换会很复杂。数学家们在研究一个理论的时候，经常会提前给出限定的条件，所以在这里，对于两个向量的转换，给出的限定条件是：

- 直线在变换后仍然是直线，lines remains lines
- 原点保持固定， origin remains fixed

从网格的这个特征来看：保持网格的平行且等距的分布，grid lines remains paralled and evenly spaced。

那么，如何来描述这些变换呢？How would you describe one of these numerically?

假设有这样一个向量：

![3-1](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/31.gif)

按照上一节的，任何一个向量可以看成是通过基向量`Linear Combination`而来的，就像这样：

![3-2](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/32.gif)

以上是在二维平面上的转换。里面的巧妙之处在于基向量中i头和j头这种`原子思维`思想的设计。

现在，来翻转一下这个二维平面，这时候定义i头和j头的方便之处就体现出来了：翻转后，原来的i头和j头依然存在，但在空间的坐标却变了，这时候的i头和j头的可以看作是transformed i和trasformed j。假设，transformed i和trasnformed j分别是：

![3-3](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/33.gif)

就是基向量的坐标变了，其它的都没变，我们还是可以获得在这个新基向量基础上`linear combination`之后得到的向量：

![3-4](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/34.gif)

也就是，在空间坐标下的transformed i和trasformed j是一个基本单元，这个基本单元就把它定义成`矩阵Matrix`。

![3-5](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/35.gif)

左边的这排代表着空间坐标系下的trasformed i,右边的这排代表着空间坐标系下的transformed j。而所有的`线性转换Linear Transform`都可以在`矩阵Matrix`基础上转换得来。

所有的`线性转换linear transform`可以抽象成如下：

![3-6](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/36.gif)

总结一下就是：在二维空间里的i head和j head, 首先翻转一下，即通过`Linear Trasform`，这个二维平面得到transformed i head和trasnformed j head，这个transformed i head和trasformed j head组成了`矩阵Matrix`，`矩阵Matrix`就是在三维坐标下的基本单元，然后通过`线性组合Linear Combination`得到了空间中的一个新向量。所以，也可以这么说：三维空间是由`矩阵Matrix`通过`线性组合Linear Combination`形成的，二维空间是由i head和j header通过`线性组合Linear Combination`形成的






