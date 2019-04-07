一个类包含基本构成。
```
public class Card
{
    protected string _name;
    protected int _attack;
    protected int _defence;

    public Card(string name, int attack, int defense)
    {
        _name = name;
        _attack = attack;
        _defense = defense;
    }

    public virtual string Name
    {
        get
        {
            return _name;
        }
    }

    public virutal int Attack
    {
        get
        {
            return _attack;
        }
    }

    public virtual int Defense
    {
        get
        {
            return _defense;
        }
    }
}
```

装饰器的基类。

```
public abstract class CardDecorator : Card
{
    //一切神奇来自这里
    protected Card _card;

    public CardDecorator(Card card, string name, int attack, int defense) : base(name, attack, defense)
    {
        _card = card;
    }

    public override string Name
    {
        get
        {
            return $"{_card.Name},{_name}";
        }
    }

    public override int Attack
    {
        get
        {
            return _card.Attack + _attack;
        }
    }

    public override int Defense
    {
        get
        {
            return _card.Defense + _defense;
        }
    }
}
```

具体装饰器。

```
public class AttackDecorator : CardDecorator
{
    public AttackDecorator(Card card, string name, int attack):base(card, name, attack, 0)
    {

    }
}

public class DefenseDecorator : CardDecorator
{
    public DefenseDecorator(Card card, string name, int defense) : base(card, name, 0, defense)
    {

    }
}
```

使用
```
Card soldier = new Card("Soldier", 25,20);
soldier = new AttackDecorator(soldier, "Sword",15);
soldier = new DefenseDecorator(soldier, "Heavy Armor",50);
```