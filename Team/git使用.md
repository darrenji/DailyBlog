> 在windows下安装git-scm, git bash

```
git --version
git config --global user.name ''
git config --global user.emal ''
git config --local user.name '' 需要在创建.git文件夹后使用
git config --local user.email '' 需要在创建.git文件夹后使用
git config --system user.name ''
git config --system user.email ''
git config --global --list
git config --global --list user.name
git config --local --list
git config --local --list user.name
pwd 当前目录
ls -al 列出当前目录下的所有文件
rm -rf 文件或文件夹名,删除文件或目录
cd 文件夹
cd ..返回到上一级
git init
clear 清除当前屏
cp ../目录/文件名 文件名 拷贝
touch file 创建文件
git add --all
git add -u
git add 文件名
git status
git commit -m ''
git commit -am ''
git rm --cached 文件名 从暂存区中删除文件
git log
mkdir styles创建目录
vi scripts.js查看文件
```

修改暂存区的文件名
```
//不推荐的写法
mv readme readme.md 修改文件名称从原来的变成新的
git rm readme 从暂存区中删除该文件
git add readme.md

//推荐写法
git reset --hard 把暂存区和工作目录都复原
git mv readme readme.md 也就是直接在暂存区修改
```

查看版本演变
```
git log 
git log --oneline
git log --oneline -n4 最近的4个
git branch -av 查看分支
git checkout -b temp commit的hash值，也就是针对某个commit创建分支
git branch -av 会看到HEAD指向temp这个分支
git log 查看某个分支上的Log，即当前分支上的log
git log --all 查看所有分支上的log
git log --all --graph 查看所有分支上的log，以图的形式展示
git log --all --oneline 查看所有分支上的log，一行展示
git log --all --graph
git log --oneline --all
git log --oneline --all -n4
git log --oneline --all -n4 --graph
git help -web log
git k打开git图形界面
```

.git目录是git的核心，这里有什么呢？
```
//HEAD文件，当前指向哪个分支
//文件内容：refs/heads/temp,当前heads指向temp分支
git branch -av 查看所有分支
cd ..
git checkout master切换分支
HEAD文件内容有所改变

//refs文件夹
//包含heads文件夹和tags文件夹
//heads里面是不同的分支，tags里面是不同的tag
比如master分支里什么内容呢？cat master
这个hash值是什么类型呢？git cat-file -t hash值， 是commit类型
查看所有分支：git branch -av, master hash值，
//tags里面是不同的tag
查看某个tag: cat js01
看看hash值是什么类型：git cat-file -t hash值,是tag类型，每个tag有一个hash值
看看hash值存的内容：git cat-file -p hash值，是commit类型,每个tag的hash值内容是commit类型，指向某个commit

//objects文件夹
//里面有多个子文件夹
cd 42 切换到42这个子文件夹
ls -al 会看到一个hash值
查看42和hash值拼接成的新hash值：git cat-file -t 42.... tree类型
git cat-file -p 42..., 会看到里面的文件是blob类型，一个文件对应唯一的blob

```

git中有三个重要的对象，分别是commit, tree, blob。commit对象

```
从某个commit开始：git branch -av
依次查下去：git cat-file -p hash值
```

.git下有几个tree?

```
git init watch_git_objects
cd watch_git_objects
ls -al
mkdir doc
git status
cd doc
echo "hello" > readme 写内容并保存到readme文件中
cd ..
find .git/objects -type f 现在.git/objects中还没有内容
git add doc
find .git/objects -type f 现在.git/objects下已经有内容，即从加入暂存区开始git就会记录
git cat-file -t ...
git cat-file -p 发现现在.git/objects下的是blob类型
git commit -m '' commit后，.git/objects下不仅有blob，还有commit, tree类型
git cat-file -t ...
git cat-file -p ...

总结：git把commit, tree, blob放在了.git/objects目录中，commit下肯定有一个tree,相当于根节点，然后根节点下会有子节点
```

分离头指针。一般HEAD指向分支，如果把HEAD指向某个commit，这就是分离头指针。分离头指针说明是现在工作在一个没有分支的情况下，如果在分离头指针的情况下产生commit，如果将来哪一天需要回到某个分支，比如master分支，这时分离头指针前提下的commit会被git丢弃。

```
git branch -av
git log --oneline
找都某个commit，然后git check out commit的hash值，这就实现了分离头指针

分离头指针，对git来说是一件需要注意的事，所以git会给到如下提示：

You are in 'detached HEAD' state. You can look around, make expreimental changes and commit them, and you can discard any commits you make in this state without impacting any branches by performing checkout.If you want to create a new branch to retain commits you create, you may do so (now or later)by using -b with the checkout command :git checkout -b <new-branch-name>

git告诉我们，在detached HEAD这个状态下，要么做一些提交，但这些提交不会影响分支。要么干脆创建新的分支。

暂时就不创建分支了。

git branch -av
修改某个文件
git commit -am ''
git log --oneline 发现HEAD不指向任何一个分支，而是指向commit
回到master分支：git checkout master

git又给出了warning:

Warning: you are leaving commit behind, not connect to any of branches, if you want to keep it by crating a new branch, this mabye a good do do with:git branch <new-branchname> commit_hash_code

git再次提醒我们要不要创建新的分支。
gitk --all打开可视化界面，我们并没有发现刚刚的commit
如果我们想给刚刚的commit创建一个分支：git branch fix-js commit_hashcode
```

HEAD和Branch
```
git branch -av
git log
创建某个分支并来到某个分支：git checkout -b fix_readme fix-js ,然后HEAD就指向了fix_readme这个分支了
验证HEAD的指向：cat .git/HEAD, 内容refs/heads/fix_readme
查看分支：cat .git/refs/heads/fix-readme,获取hash_code
查看类型：git cat-file -t hash_code,hash_code代表commit类型

以上，.git/HEAD查看到HEAD指向分支，到.git/refs/heads中找到分支，获取hash_code,查看其类型是commit。也就是HEAD指向了分支，实际HEAD指向了commit。
git diff commit_hash_code commit_hash_code
git diff commit_hash_code HEAD
git diff HEAD HEAD^1
git diff HEAD HEAD^^
git diff HEAD HEAD~2
```

删除分支
```
git branch -av
gitk --all
git branch -d fix-js
	error: The branch 'fix-js' is not fully merge.
	If you  are sure you want to delete it, run 'git branch -D fix-js'
bit branch -D fix-js
gitk --all
git branch -av
```

修改最近一次的commit的message
```
git log -1
git commit --amend
i
esc :wq!
```

修改以前的某次commit
```
git log -3
git rebase -i 要修改的前面一次父commit的hash值

reword
pick
git log -n3 --graph
```

合并多个连续的commit
```
git branch -av
git log --graph
git rebase -i 要修改的连续几次commit的前面那个父commit的hash值
i

s 
s
esc :wq!
git log --graph

```

把间隔的几个commit合并成一个
```
git log --graph
git rebase -i root_commit_hashcode这里root commit需要和其它几个commit合并
	这时的unix窗口是没有root_commit相关的，需要手动把root commit敲上去
	i
	pick root_commit_hashcode
	pick some_commit_hashcode
	pick some_commit_hashcode
把需要和root_commit合并的commit用s命令
	i
	pick root_commit-hashcode
	s some_commit_hashcode
	pick some_commit_hashcode
	esc :wq!
git status 不仅可以展示状态，有时可以看到报错，并且有git给出的建议
git log --graph
gitk --all
注意：这样处理后所有的commit就可能会不连续
```

暂存区和HEAD包含文件的比较
```
vi index.html
git add index.html
git diff --cached(如果没有--cached表示暂存区和工作区之间的比较)
	-表示删除+表示增加
git commit -m ""
```

暂存区和工作区比较
```
vi index.html
git add index.html
git diff
git diff -- readme.md ...
```

暂存区恢复到和HEAD一致，即 恢复到和当前分支一样。
```
git reset HEAD
git status
git diff --cached 暂存区和HEAD比较
```

工作区的文件恢复为和暂存区一样。
```
git add index.html
git diff --cached 暂存和HEAD的区别
vi index.hml
git diff 暂存和工作区的区别
git checkout -- index.html
git diff index.html
```

暂存区恢复到和HEAD一样。
```
git reset HEAD -- styles/style.css
```

消除最近的commit，首先确定新的HEAD指向那个commit,这个commit的hashcode记作new_commit_hashcode.
```
git reset --hard new_commit_hashcode
注意：--hard表示暂存区和工作区都恢复到指定commit相同的状态
```

比较不同commit上文件的差异。
```
git log -n8 --all --graph
git diff temp master
git diff temp master -- index.html
git diff temp_commit_hashcode master_commit_hashcode -- index.html
```

正确删除文件。
```
ls -al
用两条命令:
rm readme 删除工作区文件
git rm readme 删除暂存区文件
git reset --hard HEAD 把暂存区和工作区都恢复到和当前分支的HEAD一样的状态
用一条命令：git rm readme
```

为了做临时任务，先把工作区清空，等做完临时任务，add, commit之后再把原先的工作区内容放出来开始做。
```
git status 工作区有文件
正在修改index.html
git stash
git stash list 发现stash里有内容了
git status 这时工作区没有文件了
做紧急任务，add, commit之后
git stash apply(从stash弹出来，stash还有内容)
git stash pop(从stash弹出来，stash没有，有时和git reset --hard HEAD配合使用)
git diff 比较暂存和工作区
git stash list
```

指定不需要git管理的文件
```
ls -al
mkdir doc
ech 'hi' > doc/readhim
git status 工作区里有文件
vi .gitingore
	doc
git status
```

git仓库备份到本地
```
使用哑协议： git clone --bare /e/demos/git/git_learning/.git ya
使用智能协议：git clone --bare file:///e/demos/git/git_learning/.git zhi
来到git_learning中查看远程git:git remote -v
还没有远程git:git remote add zhi file:///e/demos/git/backup/zhi
查看git_learning中的分支：git branch -av
把git_learning中的内容推到远程：git push  --set-upstream zhi master
```

配置公钥私钥
```
查看是否已经有公钥私钥：C:\Users\Administrator\.ssh
如果没有，打开git bash终端：ssh-keygen -t rsa -b 4096 -C "your_email@example.com"
	id_rsa私钥
	id_rsa.pub公钥，放在共网
拷贝公钥到https://github.com/settings/keys	
```