> 从内存上来看，数组在内存上有一块连续的内存地址，而链表的内存地址不是连续的。为什么需要链表呢？

那数组来说，获取数组元素的时间复杂度是O(1)，但是数组元素的插入、删除时间复杂度是O(n)，数组为了保证内存地址的连续性，需要做大量的数据搬移，效率就降下来了。

而在数据的插入和删除方面，链表的效率高得多，时间复杂度是O(1)。拿单链表来举例。

> 单链表。单链表的节点不仅有数据，还包含了一个指向下一个节点的指针。单链表的最后一个节点，即尾部节点指向为Null，指向到一个空地址。

先说插入节点。假设单链表有3个节点a,b,c,现在把节点x插入到b之后。首先把b的指针指向x，再把x的指针指向c，就这样插入完成了，时间复杂度是O(1)。

再说删除节点。假设把上面的b节点删除。首次把a节点的指针指向null，b节点的指针指向null，再把a节点的执行指向c，删除完成，时间复杂度同样是O(1)。

> 除了单向链表，还有单向循环链表、双向链表、双向循环链表。

所谓的循环是指最后一个节点的指针指向了第一个节点。所谓的双向是指一个节点中有2个指针，一个指针指向前面一个节点，另一个指针指向后面一个节点。

> 链表在缓存策略中的应用。缓存策略中，有一种叫做“LRU缓存策略”，即Least Recently Used,把最近使用到的放在缓存列表的头部。

这个缓存策略中维护者所有的节点、第一个节点、最后一个节点、已经容量。

```

 public class LRUCache
    {
        private int _capacity;
        private Dictionary<int, Node> data;
        private Node head;
        private Node end;

        public LRUCache(int capaicity)
        {
            _capacity = capaicity;
            this.data = new Dictionary<int, Node>();
        }
	}

public class Node
    {
        public int key { get; set; }
        public int data { get; set; }
        public Node previous { get; set; }
        public Node next { get; set; }
    }
```

接着`LRUCache`下的运行逻辑。

首先是插入节点。要重置插入的节点，所谓的重置即把节点的前驱指向和后驱指向都设置为null。然后呢，可能插入的时候，头部节点还没有，这时缓存策略的head, end节点就是插入的节点。最后就是正常的插入节点，由于插入到链表的头部，把头部节点的指针指向插入的节点，把插入节点的后驱指针指向头部节点，再把头部位置交给新插入的节点。

```
        public void AddNode(Node node)
        {
            //重置需要插入的节点
            node.previous = null;
            node.next = null;

            //如果还没有头部结点，即头部节点为Null
            if(this.head==null)
            {
                this.head = node;
                this.end = node;
                return;
            }

            //如果已经有了头部节点，就把新插入的节点放到最前面
            this.head.previous = node;

            node.next = this.head;
            this.head = node;
        }
```

然后是删除节点。考虑删除节点是否为null,head节点是否为null, 如果剩下最后一个节点，如果删除head节点，如果删除end节点，最后就是删除head和end之间的节点了。

```
public void RemoveNode(Node node)
        {
            //先考虑没有的情况
            if (this.head == null || node == null) return;

            //如果只剩下最后一个节点了
            if(this.head==this.end&&this.head==node)
            {
                this.head = null;
                this.end = null;
                return;
            }

            //删除头部节点
            if(node==this.head)
            {
                this.head.next.previous = null;
                this.head = this.head.next;
                return;
            }

            //删除尾部节点
            if(node==this.end)
            {
                this.end.previous.next = null;
                this.end = this.end.previous;
                return;
            }

            //其它情况
            node.previous.next = node.next;
            node.next.previous = node.previous;
        }
```

经常做的一个动作是，把链表中的其中一个节点先从链表中删除，再放到链表的头部。

```
        public void RemoveAndMovieToFirstNode(Node node)
        {
            this.RemoveNode(node);
            this.AddNode(node);
        }
```

当缓存满的时候，需要删除最后一个节点。

```
        public void RemoveLastNode()
        {
            this.RemoveNode(this.end);
        }
```

最后就是缓存的读取和设置了。先是读取。内部执行了先从链表中删除再放到链表头部。

```
        public int GetNode(int key)
        {
            Node n;
            if(this.data.TryGetValue(key, out n)==true)
            {
                this.RemoveAndMovieToFirstNode(n);
                return n.data;
            }
            else
            {
                return -1;

            }
            
        }
```

最后就是设置缓存。需要考虑字典集合里是否有，是否超出范围。

```
        public void SetNode(int key, int value)
        {
            Node n;
            if(this.data.TryGetValue(key, out n)==true)//如果在字典集中存在
            {
                //放到缓存链表的头部去
                this.RemoveAndMovieToFirstNode(n);
                n.data = value;
                return;
            }

            //超出范围
            if(this.data.Count>=this._capacity)
            {
                int id = this.end.key;

                //同时在字典集和缓存列表里删除
                this.RemoveLastNode();
                this.data.Remove(id);
            }

            //普通情况
            Node node = new Node();
            node.key = key;
            node.data = value;
            this.AddNode(node);
            this.data.Add(key, node);
        }
```

以上，字典集合的存在省去了很多查找时间，链表本来查找的时间复杂度是O(n)，有了字典集，时间复杂度变成了O(1)，再加上链表的拿手好戏插入和删除，整个的效率是非常高的。