前几节中以i head, j head, k head为基点，然后进行二维和三维空间上进行一次或多次linear transformation，可以把向量看作是基于基向量的scaling。在本篇中，要谈的话题是`determinant`。这是一个什么概念呢？

当在二维平面，i head 和 j  head的乘积是1平方单位，在三维平面时i head, j head, k head的乘积时1立方单位，而在进行二维和三维线性转换的时候，i head和j head围成的平面面积以及i head, j head, k head围成的三维面积都在改变，为了描述这种相对于基二维面积和基三维面积的这种改变，引入了`determinant`这个概念。

如果是求二维的`determinant`，那就是：
![6-1](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/61.gif)


如果是求三维的`determinant`，那就是：
![6-2](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/62.gif)