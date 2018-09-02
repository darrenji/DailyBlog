> 在前几节中，首先是让二维平面静止下来，然后用`原子思维`定义了`i head`和`j head`，这是在二维平面上的基本单元，也叫`基向量`。有了这个基本单元，所有的向量就可以看作是在`基向量`基础之上的`缩放Scaling`,而向量的值可以看作是`标量Scaler`。在上一节中，二维平面开始有了转动，这个转动引起了`基向量`的变化，也就有了`transformed i head`和`transformed j head`之说，这个`transformed i head`和`transformed j head`构成了`矩阵Matrices`,这个`矩阵Matrices`就是空间变化的基础。在这一节里，要面对的是二维平面的多次转动。

假设有这样一个变量。

![2-1](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/21.gif)

首先来看第一次二维平面的转动。一旦有转动就得到了一个新的矩阵，而原先的向量在新的坐标下的位置是什么呢？就是在新的矩阵下进行缩放，如下：

![4-1](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/41.gif)

对`trasformed i head`和`transformed j head`分别缩放，而`trasformed i head`和`transformed j head`都有自己的x,y坐标，把x坐标和y坐标下进行`线性组合`得到如上的结果。

接着第二次二维平面转动，又有了新的矩阵，再次计算在新的坐标系的向量坐标，如下：

![4-2](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/42.gif)

两次转动可以概括为：

![4-3](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/43.gif)

也就是矩阵的相乘实际上就是二维平面的转动得到新的向量坐标。

所以,

```
M1M2和M2M1是不相等的，因为转动的方向不一样。
```

而

```
A(BC)=(AB)C, 因为转动的次序是一样的。
```






