Git是分布式的Version Control System, 其本质是Content-Addessable File System, 存储键值对，通过键取出内容。

构成：

- 工作区，Worker Space
- 暂存区，Indes Stage
- 本地仓库，Local Rpository
- 远程仓库， Remote Repository

关系：

- 工作区通过`add`进入暂存区
- 暂存区通过`commit`进入本地仓库
- 本地仓库通过`push`进入远程仓库
- 远程仓库通过`clone`,在本地文件夹clone把远程仓库拷贝到本地文件夹
- 工作区通过`pull`把远程仓库拉到本地工作区，`pull`的过程包括`fetch` + `merge`

Git命令实际上有两种层级：

- procelain命令，一些常用的命令就是这种类型的命令
- plumbing命令，这是一些更底层的命令

常用命令：

```
git --version
git config --global --list
git config --local --list
pwd
ls -al
rm -rf
cd ..
cd /
clear
```

.git文件夹是很重要的文件，默认处于隐藏状态。

```
config文件：项目配置信息
    是否以bare方式初始化， bare = false
    remote信息 url=git@github.com:username/name.git,就是通过git remote add输入进去的远程地址
objects文件夹：包含了git对象
    手动创建对象 echo 'test' | git hash-object -w --stdin, -w若不指定返回对象的键值， --stdin若不指定需要指定文件的存储路径
    默认文件夹 pack文件夹存储打包压缩对象 info存储pack文件夹中的查找对象
    查看git对象的存储值 find .git/objects -type f 是一个40个字符，SHA-1哈希值，通过存储数据+头信息检验和得出，40个字符的前两位字母作为objects子文件夹的名称，后38个字符作为文件名存在objects下两个字符开头的文件夹的下面
    查看存储对象文件夹列表 find .git/objects
    查看对象内容 git cat-file -p 40个字符
    手动创建文件 echo 'version1' > test.txt
    把文件存储为对象 git hash-object -w test.txt
    再次修改文件 echo 'version2' > test.txt
    再次把文件存储为对象 git hash-object -w test.txt
    查看object下的所有对象 find .git/oobjects -type f
    将文件恢复到第一个版本 git cat-file -p 第一个版本40个字符 > test.txt
    查看内容 cat test.txt
    将文件恢复到第二个版本 git cat-file -p 第二个版本的40个字符 > test.txt
    查看内容 cat test.txt
    查看对象类型 git cat-file -t 40个字符
tree树对象，类似于UNIX中的文件系统，存储blob对象或者tree对象
commit对象，通过`git commit`上去的对象
HEA文件， refs/refs/heads/feature/some_commit,
    当前活跃分支的游标
    git用它来追踪当前位置
    HEAD可以指向任何一个节点
    如果HEAD指向没有分支名的版本叫DETACHED HEAD
    head是commit对象的引用，每个head都有一个名字，标签名或者分支名
    如果head被选择为current head, 这个head就变成了HEAD
index文件：暂存区的信息
refs文件夹：heads文件夹每个分支最后一次的commit, remotes文件夹最后一次和远程仓库的通讯，tag文件夹
```

分支管理

```
mkdir tutorial
cd tutorial
git init
ech 'hello' > myfile.txt
git add myfile.txt
git commit -m ""
这时候HEAD指向master分支的第一个commit

创建新分支：git branch issue1(git checkbout -b issue)
git check out issue1
修改myfile.txt
git add myfile.txt
git commit -m ""
此时HEAD指向issue1分支的第一个commit

合并分支
git merge <commit> commit被合并到当前分支上
大多时候需要把一个分支合并到活动分支上 git merge <not_active_branch_name>
把issue1上的commit合并到master: git checkout master, git merge iisue1
现在HEAD指向master最新的commit上

删除分支：git branch -d issue1

在多个分支下工作
git branch issue2
git branch issue3
git checkout issue2
修改myfile.txt
git add myfile.txt
git commit -m ""
此时HEAD指向issue2的commit上
git checkout iisue3
修改myfile.txt
git add myfile.txt
git commit -m ""
此时HEAD指向issue3的commit上
现在想把iisue2和issue3合并到master分支上
git checkout master
git merge issue2
现在HEAD指向master分支上原来是issue2分支上的commit
git merge issue3
    CONFLICT  (content): Merge conflict in myfile.txt automatic merge failed, ;fix conflicts and then commit the result
    conflict:同一行有不同的内容
手动解决conflict
git add myfile.txt
git commit -m ""
现在HEAD指向master分支上新的、手动解决后的commit

通过rebase方式解决conflict:即把issue3整体放到master分支的最后面，master有了新的基
git reset --hard HEAD~
git checkout issue3
git rebase master
手动解决conflicts
git add myfile.txt
git rebase --continue
git checkout master
git merge issue3
```

实用创建分支

```
git branch
git branch -av
git log --oneline -n20
基于某个commit创建分支：git checkout -b new_branch hash_code
先创建分支再切换：git branch new_branch, git checkout new_branch
创建全新的分支：git checkout -b new_branch
```

tag管理
```
mkdir turorial
cd tutorial
git init
outch myfile.txt或echo 'hello' > myfile.xtx
git dd myfile.txt
git commit -m ""
创建新的tag:git tag apple
查看tag:git tag
查看tag的历史信息：git tag --decorate
给tag添加注释：git tag -am ""
查看带有注释的tag： git tag -m
删除tag： git tag -d apple
```