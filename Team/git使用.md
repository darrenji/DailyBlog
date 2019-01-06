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

gighub创建个人仓库，把本地同步到远端
```
选择Owner,可以是组织或个人
勾选Initialize this repository with a README，这里的描述和github搜索相关
.gitignore,可以选择某个模板
协议，比如MIT License属于免费分享(这个文件在本地还没有哦)
查看remote:git remote -v
删除分支：git remote remove 分支名称
添加远程地址：git remote add 名称 git@github.com:git.....git
git remote -v
把本地所有的分支上传到远程：git push 名称 --all
	![rejected] master-->master(fetch first)
	error:failed to push some refs to 'git@github.com:git.../git_example.git'
	也就是没有基于远端作变更
gitk --all
	如果分支已经成功同步到远端：remote/名称/分支名
	由于远端预先创建了license文件，所有远端没有：remote/master
现在远端的master分支有问题，先把远端master分支fetch下来：git fetch 名称 master(git pull包含两个动作，fetch+merge)
gitk --all
	多出了remote/名称/master, 里面有远端预先创建的licence文件
	本地的master
    现在要解决不是fast-farward的问题，有两种解决方式：rebase或者merge
查看本地分支：git branch -v
查看本地和远端分支：git branch -va
切换到master分支：git checkout master
本地的master和远端master合并：git merge 远端名称/master(这样写不行：远端名称 master)
	fatal:refusing to merge unrelated histories,也就是本地的master和远端的master不相干，是两个独立的树
查看merge命令，git merge -h 
再来忽略历史的一个merge命令：git merge --allow-unrelated-histories 远端名称/master
gitk --all
现在远端和本地已经merge了，把本地的Push: git push 远端名称 master(本地的master分支)
gitk --all
	这时本地的master和远端的master都指向同一个commit了：master--remotes/远端名称/master

总结一下：
git remote -v
git remote remove remote_name
git remote add remote_name(远端默认名称是origin) ...
git remote -h
gitk --all
git branch -v
git branch -va
git checkout master
git fetch remote_name master
git merge --allow-unrelated-histories remote_name/master
git push remote_name master
```

在同一个分支中，不同人修改不同文件
```
在github上创建分支：feature/add_git_commands,在切换分支处输入名称回车即可
在本地clone一个：git clone git@github.com:darrenji/git_learning.git git_learning_02
ls -al
修改新创建文件夹git_learning_02里本地用户：git config --add --local user.name 'git2019' 
查看本地配置：git config --local -l
git config --add --local user.email 'git2019@qq.com'
git config --local -l
	如果要修改配置文件：vi .git/config, i, esc, :wq!
查看所有分支：git branch -av
基于远端的分支创建本地分支并切换到本地新创建分支：git checkout -b feature/add_git_commands origin/feature/add_git_commands
git branch -v
vi index.html, i, esc, :wq!
git add -u 
git commit -m 'update index by git2019'
git push (feature/add_git_commands-->add_git_commands)(因为创建本地分支的时候和远端的分支挂钩了起来，并且现在本地在刚创建的新分支上)(git push origin feature/add_git_commands)
回到原先的git_learning
git config --local -l
查看所有的分支，git branch -av，发现刚刚创建的分支还没到这里的本地
查看远端的名称：git remote -v
把远端fetch下来：git fetch gitlearing
	*[new branch] feature/add_git_commands -> gitlearing/feature/add_git_commands
git branch -v发现远端的新分支在本地还是没有
git branch -av用这个名利是可以看到远端的新分支的
现在要创建本地分支：git checkout -b feature/add_git_commands gitlearing/feature/add_git_commands
bit branch -av现在本地已经有远端新创建的分支了，而且本地和远端新创建分支的hash值是一样的
vi styles/style.css, i, esc, :wq!
git add -u
git commit -m 'update styles.css by darren'
gitk --all,发现本地的feature/add_git_commands和reotes/gitlearing/feature/add_git_commands是fast foward关系
	这时另一个用户又想修改
来到git_learning_02
git branch -av
vi index.html,i, esc, :wq!
git commit -am 'update second time by git2016'
git push
再次来到git_learning文件夹，cd .. | cd git_learning
git branch -av 此时git_learning这个用户不知道远端又作了变更
git push gitlearing
	Updates were rejected because the remote contains work that you do not have locally. This is usually caused by another repository push to the same ref.You may want to first intergrate the remote changes before pushing again.
先把远端fetch下来：git fetch gitlearing
git branch -av
	本地分支feature/add_git_commands hash_code [ahead 1, behind 1]，是说本地分支有一个比远端新，有一个比远端旧
git merge gitlearing/feature/add_git_commands,因为两个用户修改的是不同文件，对git来说很容易解决
git push gitlearing
```

不同的人修改同一个文件的相同区域
```
来到git_learning_02文件夹
git branch -av 查看所有分支和当前分支
git pull(fetch+merge)
vi index.html
git commit -am 'add by git2019 again',注意这里还没有push
来到git_learning文件夹
vi index.html
git commit -am 'add by darren again'
git push gitlearing
来到git_learning_02文件夹
git push
	failed to push some refs to ...updates were rejected
git fetch
查看分支情况：git branch -av
	feature/add_git_commands  hash_code[ahead 1 behind 1]
	remotes/origin/feature/add_git_commands hash_code add by darren again.
git merge origin/feature/add_git_commands
	auto-merging index.html
	CONFLICT(content):Merge conflict in index.html
	Automatic merge failed; fix conflics and then commit the result
手动修改index.html文件
cat index.html
git status
git commit -am 'resolve commit by git2019'
git push
来到git_learning文件夹
git pull gitlearing
```

不同的人修改同一个文件的不同区域。
```
来到git_learning_02文件夹
git branch -av
git pull 保证和远端保持更新
gi branch -av
vi index.html
git commit am 'git2019 in one area' 注意这里还没有push
来到git_learning文件夹
vi index.html
git commit am 'darren in this area'
git push gitlearing这时，git2019还不知道这里已经有了push
来到git_learning_02文件夹
git push
	error:failed to push some refs to 
    也就是现在的状态不是fast forward
git fetch 此时fetch下来还没有merge
git branch -av
	feature/add_git_commands [ahead 1, behind1]ahead是因为这里提交还没有push,behind是因为darren用户提交到远端的这里还没有同步过来
git merge origin/feature/add_git_commands
直接进入index.html手动修改
cat index.html
gitk --all
git branch -av
git status
git commit -am 'resolve'
git branch -av 
	feature/add_git_commands [ahead 2]就是比远端还领先，可以git push
git push
git branch -av 发现feature/add_git_commands没有ahead,behind了，local和remote的分支的hash_code一样
来到git_learning文件夹
git pull gitlearing
cat index.html
```

同时变更了文件名和文件内容。
```
来到git_learning文件夹
git branch -av
git pull gitlearing
来到git_learning_02文件夹
git pull
来到git_learning文件夹
ls -al
git mv index.html index.htm
git status
git commit -am 'change index name'
来到git_learning_02文件夹
ls -al
vi index.html
git commit -am 'modify index title'
来到git_learning文件夹
git push gitlearing
来到git_learning_02文件夹
git push
	error:failed to push some refs
git pull
	unix窗口,这里看到了git的神奇之处，它能把index.html的改名和内容更改merge成一步
ls -al
	文件名已经变成了inex.htm
cat index.htm
```

多人修改了文件名
```
来到git_learning文件夹
git pull
来到git_learning_02文件夹
git pull
ls -al
git mv index.htm index1.htm
git commit -m 'change index to index1'
来到git_learning文件夹
ls -al
git mv index.html index2.html
git commit -am 'change index to index2'
来到git_learning_02文件夹
git push
	error:failed to push some refs to
git pull
	Automatic merge failed;fix conflicts and then commit the result.
ls -al
	index1.html
	index2.htm
diff index.html index2.htm
git status
	both deleted:index.htm
	added by us: index.html
	added by them: index2.htm
git rm index.htm
git status
	added by us: index1.html
	added by them: index2.htm
git add index1.html
git status
	added by them:index2.htm
git rm index2.htm
git status
	Your branch and 'origin/feature/add_git_commands' have diverged,
	and hav 3 and 1 different commits each, respectively.
	(use "git pull" to merge the remote branch into yours)
	All conflicts fixed but you are still merging
	(use "git commit" to conclude merge)
git commit -am 'Decide to mv index to index1'
git status
gitk --all
git branch -av
	[ahead 4]
git push
ls -al
这时github上也是index1.html
```

fastforward和non-fastforward
```
假设远端的名称是ariya
现在为远端创建分支：git checkout -b speedup

现在先fetch后merge，默认使用的是fast-forward模式，会删除speedup分支然后合并到master分支上。这种做法适合master和合并commit之间没有master的commit.
git fetch ariya
git merge ariya/speedup

还有一种情况是，保持分支不动，当master和合并commit之间有commit，或者想故意保留其它branch。
git fetch ariya
git merge -no-ff ariya/speedup
```

禁止向集成分支执行push-f操作
```
来到git_learning_02文件夹
git branch -av
git log --oneline
现在把当前分支的HEAD指向master HEAD指向commit之前的某个分支：git reset --hard 5a7e69b,这样当前分支上5a7e69b之前的commit都没有了，本地没有了
推到远端强制同步：git push -f， 也就时不采用fast-forward模式了
此时github上,feature/add_git_commands分支上的commit少了很多，这在团队开发中会产生不好的影响
实际上在github上是可以设置不能进行git push -f操作的
```

merge和rebase。
```
场景：从master的某个commit开始多处了一个branch叫做Feature,然后这个Feature分支不断地有commit,Master分支也不断有commit.

Merge
git checkout Feature
git merge master
或者
git merge Feature master
这样Feature分支上有了master分支的历史，有时看起来比较混乱

Rebase
git checkout Feature
git rebase master
这样master作为基，Feature接着master，就会得到一个新的、线性的commits

如果rebase的时候想控制Feature上的commits呢？
git checkout Feature
git rebase -i master
	pick 33d5b7a Message for commit #1
	pick 9480b3d Message for commit #2
	pick 5c67e61 Message for commits #3

	pick 33d5b7a Message for commit #1
	fixup 9480b3d Message for commit #2
	pick 5c67e61 Message for commits #3
	
注意：不要在公共分支上使用rebase,因为，rebase后push产生新的分支，已经脱离了原来的master,而其它开发者还在master分支。

rebase后的push:
rebase后使用git push命令是不行的，需要使用：git push --force

虽然rebase需要在公共分支上禁止使用或者慎用，但是在作本地清理中很有用。
比如各项功能有各个分支。
rebase就可以在分支之间或者分支上的commit之间用上。
git checkout Feature
git rebase -i HEAD~3
```

搜索
```
https://github.com/search
https://help.github.com/
created:>2019-01-01
git 最好 学习 资料 in:readme
blog easily start in:readme stars:>5000
git 最好 学习 资料 in:readme stars:>1000
filename:.gitlab-ci.yml
```

组织性仓库
```
Settings--Organizations 
Organization组织的概念
Organization有People管理
Organization有Repositories
Organization有Team管理
	Team下有Member管理,Member相当于用户
    Team管理哪些仓库，对仓库的读写权限
	Team下有角色，角色管理哪些仓库，对仓库有哪些读写权限
	一个新的Member进来没有写权限但又读权限，可以看到Team, Member， Repository等
```

创建团队项目
```
create a new repository
	owner,Repository name, Description, public, ignore
	Grant your Marketplace apps access to this repository,勾选以前用过的持续化集成CI或者代码覆盖率的的Marketplace上的服务
Settings-Collaborators & teams
	设置Team的权限
```

选择团队工作流
```
考虑的因素：人员组成、研发能力、产品特征是云平台还是APP、难易程度
主干开发：适用于成员能力强，人员少，沟通顺畅，用户升级组件成本低，有一套有效的特征切换的实施机制，保证上线后无需修改代码就能够修改系统行为，需要快速迭代，想获得CI/CD的所有好处。
Git Flow:不太适合敏捷开发团队，不具备主干开发能力，有预定的开发周期，需要执行严格的发布流程。
GitHub Flow:不具备主干开发能力，随时集成随时发布，分支集成时经过代码评审和自动化测试，就可以立即发布的应用。
GitLab Flow:有Master和Production分支，不具备主干开发能力，无法控制准确的发布时间，但又要求不停地集成。
GitLab Flow带环境分支：Master, Pre-Production, Production分支，不具备主干开发能力，需要逐个通过各个测试环境的验证才能发布。
GigLab Flow带发布分支：Master分支，同一个分支有多个版本，比如设备不太变，软件一直在变，可以用一个master多个分支，多个分支会长期存在。
```

分支分集成策略
```
Insights--Network,看到各个分支的发展情况
settings--Options--Merge button
	Allow rebase merging:希望master是一条清晰的线性线，每一次特性分支合并到master都让master最新的Head作为rebase,最后可以把特性分支删除。
Pull requests--New pull request
选择master和特性分支，特性分支指向master分支
Create pull request按钮
	Merge pull request,提交后，还提供删除按钮
Insights--Network,现在可以看到当把特性分支merge到master的时候，会创建一个新的commit
以上通过Merge pull request完成了特性分支和master的merge,并在master分支上多了一个commit。

现在需要回退
git branch -av
	发现工作在master分支上
git push -f orgin master实际不允许这样做，不允许回退
	这样Insights--Network中的master回到了merge之前的commit

Squash and merge,把特性分支上的几个commit squash成一个commit,再放到master上去
在master上会多出一个新的commit

Rebase and merge,特性分支上的所有commit会加到master分支的最前面。

总结：
rebase and merge, squash and merge适合线性
merge适合很多分支的merge
```

issue跟踪需求、任务、Bug
```
在Settings里启用，设置模板
label
有几个简单的状态
可以@某个人或团队
```

project看板管理issue和pull requst
```
Project--Create a prject
把Issue可以放到projects
可以把todo拖动不同的区域
```

CodeReview:不允许未经review的代码集成到分支
```
Settings--Branches--Branch protection rule--Add rule
```

多分支集成
```
pull request,选择某个特性分支合并到master分支
选择create a merge commit,看到紫色背景说明提交pull request成功了
在Insights的Networking中看效果,master分支上多一个一个commit,这个commit有两个父亲，一个是master上它的上一个节点，一个是特性分支上的上一个节点。master上的新commit叫merged commit。

另外一个特性分支也提交pull request，发现冲突，点击解决， mark as resolved,注意这里的解决冲突是在当前分支中解决的，即把master分支merge到了当前的分支中，这时特性分支和master分支属于fast forward关系了,即master在特性分支的最前面，即特性分支又向前走了一个commit

另一个分支继续提交pull request,选择create a merge commit,特性分支再次与master合并，master分支又向前了一个commit，

现在master分支需要强制回退，git push -f origin b3bf033:master(实际场景不推荐)
另一个特性分支也需要强制回退，git push -f origin 6ac0f:另一个特性分支名称(实际场景不推荐)


一个特性分支使用squash的方式提交pull request。
把特性分支上的多个commit合并成一个再与master分支合并，master分支多了一个新的commit,特性分支不变。
另一个特性分支也使用squash的方式提交pull request,解决冲突，marked as resolved, commit merge。因为是在另一个特性分支上解决冲突的，所有master分支被拉到另一个特性分支上来，另一个特性分支又向前走一步多了一个新的commit。点击Squash and merge,上次使用merge方式的话，会在master分支上多一个新的commit并指向另一个特性分支的最后一个commit，但是现在变了，master分支上也多出了一个新的commit，但是这个commit并不指向另一个特性分支。

再来看看rebase的情况。
一个特性分支采用rebase的方式提交pull request,特性分支上的所有commit会放到master分支的最前面，master原来的commit变成rebase的基点了，而且当前特性分支和master分支是fast forward关系，因为当前分支最老的那个commit的父commit是master分支上的最前面节点，所有rebase合并后一点问题都没有。
另一个特性分支也想合并到master分支上，解决冲突，master会和另一个特性分支合并，在另一个特性分支上多了一个新的合并commit。提交rebase方式的pull request。github就无法再进行下去的。另一个特性分支回退：git branch -av;git push -f origin 远程分支名称。

rebase放在本地做，方法一：
把远端拉下来：git fetch origin
git branch -av
现在另一个特性分支需要基于远端orgin/master做rebase:git rebase origin/master
找到冲突的文件：解决冲突
把刚才解决完冲突的文件放入staged cache:git add .
git rebase --continue,还会有冲突，继续解决
git add .
git rebase --continue,还会有冲突，继续解决
git add .
git rebase --continue
gitk --all也就是本地的分支已经完成rebase了
git push origin 远端另一个特性分支名称，报错，不是fast forward
git push -f origin 远端另一个特性分支名称,多出了新的分支，分支上包括所有另一个特性分支上的commit
通过rebase方式重新提交pull request,master分支上又多出了新分支上的所有commit，但是master的Head并没有指向新分支的最新一个commit。原本新的分支和master分支是fast forward关系，即新的分支的第一个commit从master的最新一个commit开始，可是为什么master的HEAD不直接指向新分支的最新一个commit(因为如果这样做，就会把谁合并提交的信息丢掉了),而是在master上又向前了一步，把新分支所有的commit放到了master上呢？而且看项目的commit，可以看到commit的author和committer,author是原先的作者，版权属于他，commiter是刚才合并提交的账号。

rebase放在本地做，方法二：先让另一个特性分支和master分支合并，并解决冲突
git branch -av
git reset --hard origin/s回到远端另一个特性分支的commit
git branch -av
git checkout master
git reset --hard hash_code
git config --global rerere.enabled true
git checkout 另一个特性分支
git merge master
解决冲突
git add .
git commit -m ''
git log -n3
git rest --hard HEAD~1
git log -n3
gitk --all
git branch -av
现在另一个特性分支需要和远端rebase:git rebase hash_code,内部知道怎样解决冲突，已经解决冲突了
git rebase --continue
git add some_conflict_file
git rebase --continue
git add --some_conflict_file
git rebase --continue
git add --some_conflict_file
git rebase --continue
```

怎样保证集成的质量
```
禁止branch提交：Settings--Branches
勾选Require status checks to pass before merging
勾选Reqire branches to be up to date before merging
勾选Market place上的持续化集成、代码覆盖率等服务
Settings--Installed GitHub Apps
```

怎样把产品发布到github上，以二进制发到github
```
.travis.yml配置
Settings--Integrations & Services
找到集成服务跳转过去
有一个token在setting--token中取
merge到master分支
release下面有包和soure code.
```

给项目增加详细文档wiki
```
把别人写得好的wiki放到自己的git上修改学习
git remote -v
gitk --all
git branch -av
git reset --hard  hash_code
git push second master
git branch -av
```

为什么喜欢GitLab?
```
自管理git,公司自维护代码平台
基于ruby on rails
有些人还会做二次开发
GitLab上有几千人维护，世界上任何一个角落的人可以修改GitLab
不停地有新功能加进来
自己搭建环境
没有使用壁垒
CI发展很快，很稳定
```