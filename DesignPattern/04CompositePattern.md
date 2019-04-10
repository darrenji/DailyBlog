复合模式适合于树状结构。有一个抽象基类，然后复合类不仅自己继承于这个基类还可以对这个抽象基类进行操作(添加删除等)，还有一个抽象类的实现，相当于叶子结点。

首先一个抽象类。
```
public abstract class GiftBase
{
    protected string name;
    protected int price;

    public GiftBase(string name, int price)
    {
        this.name = name;
        this.price = price;
    }

    public abstract int CalculateTotalPrice();
}
```

把针对抽象基类的操作封装在接口。
```
public interface IGiftOperations
{
    void Add(GiftBase gift);
    void Remove(GiftBase gift);
}
```

复合类自己派生于抽象基类，同时实现接口进行操作。
```
public class CompositeGift : GiftBase, IGiftOperations
{
    private List<GiftBase> _gifts;

    public CompositeGift(string name, int price) : base(name, price)
    {
        _gifts = new List<GiftBase>();
    }

    public void Add(GiftBase gift)
    {
        _gifts.Add(gift);
    }

    public void Remove(GiftBase gift)
    {
        _gifts.Remove(gift);
    }

    public override int CalculeTotalPrice()
    {
        int total = 0;
        foreach(var gift in _gifts)
        {
            total += gift.CalculateTotalPrice();
        }
        return total;
    }
}
```
叶子结点。
```
public class SingleGift : GiftBase
{
    public SingleGift(string name, int price) : base(name, price)

    public override int CalculatTotalPrice()
    {
        return price;
    }
}
```

使用
```
var root = new CompositeGift("",0);
var level1Child1 = new SingleGift("",1);
var level1Child2 = new SignleGift("",2);
root.Add(level1Child1);
root.Add(level1Child2);

var level1ParentNode = new Composite("",0);
var level2Child = new SingleGift("",1);
level1ParentNode.Add(level2Child);
root.Add(level1ParentNode);
```