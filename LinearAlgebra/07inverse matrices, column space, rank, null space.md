线性代数的美妙之处在于用数学表达了空间关系，是表达空间的一种语言。看下：

![7-1](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/71.gif)

以上，是两个`matrix`相乘，得到的一个新的向量。我们既可以看作是基于基向量的缩放，也可以看作是对后一个向量实施linear transformation。在脑海里有这样的一个画面：在一个二维空间里，有一个transformed i hat和一个transformed j hat，然后i方向上没有缩放只是方向和原先的trasformed i hat想法，向量的方向指向了对角线的方向。然后接着向量箭头，j方向来了一个缩放，于是通过linear combination就得到了一个新的向量。

再来看一个叫做Linear System of Equation的等式：

![7-2](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/72.gif)

脑海中的画面是这样的：在三维平面中有一个基向量：

![7-7](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/77.gif)

分解开就是：

![7-3](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/73.gif)

i方向上的缩放标量是x.

![7-4](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/74.gif)

y方向上的缩放标量是y.

![7-5](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/75.gif)

z方向上的缩放标量是z.

最后得到一个新的向量：

![7-9](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/79.gif)

用等式表示就是：

![7-5](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/76.gif)

这个等式如何称呼呢？

![7-5](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/76.gif)

以上是coefficients。

![7-8](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/78.gif)

以上是variable.

![7-9](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/79.gif)

以上是constants。

这样还不够抽象，意思是说一个向量通过某种作用就得到了一个新的向量，可以表示为：

![7-10](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/710.gif)

那反过来，v向量向着相反的方向，可以得到下面这个等式：

![7-13](https://github.com/darrenji/DailyBlog/blob/master/LinearAlgebra/713.gif)

于是就得到了本篇的第一个概念`inverse matrix`，实际上是描述两个向量关系的。一个向量通过某种作用了得到一个新的向量，相反，这个新的向量通过相反的方向也可以达到原先的那个向量。想象一下画面就很简单，两个向量通过转动、缩放可以互相得到。

但是不是所有的两个向量都是互相得到的。比如在二维平面中，一个向量转动缩放最后成就了det是0的情况，这种情况是不可逆的。从画面的角度，当一个向量在二维平面内进行线性转换det=0，反过来，很多的向量都可以通过转动缩放来到det=0的情况；从函数的角度，最终函数的输出是det为0,但是，得到det=0的输入向量有很多种情况，这完全不符合函数一个输入一个输出的原则。同样放到三维平面种，如果得到det=0的情况，向量之间也是不可逆的。

这里还要复习一下det=0的含义。意思是说从与基向量平衡的角度看，如果看不到空间，就是det=0的情况。

> rank是什么呢？

如果说det是从空间的角度来看线性转换后的结果，那么rank是从维度的角度来看线性转换的结果。如果在二维或三维空间种经过线性转换得到的是一条线，我们叫做a rank of one。如果线性转换的结果是一个平面，我们叫做a rank of two。如果一个三维空间经过线性转换得到的一个向量det不等于0，我们叫做a rank of three。好吧，rank就是从维的角度描述转换的效果。

> column space是什么呢？

det是从空间的角度描述线性转换后的效果，rank是从维的角度来描述线性转换后的效果，我们有没有想过：通过线性转换有可能得到三维、二维、线、甚至是点，我们把这种线性转换后可能性的集合叫做column space.所以column space 不是space,而是可能性的集合。

> null space是什么呢？

有一个原点(0,0)无论通过它的是直线，还是平面，这直线或平面都叫做null space.















