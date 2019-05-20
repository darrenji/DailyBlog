- 查看分支： 
```
查看本地分支：git branch 
查看本地和远程分支：git branch -av
查看远程分支： git branch -r
```
- 创建分支、切换分支
```
创建分支：git branch your_branch
切换分支：git checkout your_branch

创建并切换分支：git checkout -b your_branch(比如当前在master分支上)

基于某个commit创建并切换分支：git checkout -b your_branch commit_hash_code


```
- 删除分支
```
git branch -d your_branch
```
- 查看历史
```
git log --oneline -n10
```
- 将本地新创建的分支上传到远程服务器
```
git push -u origin your_branch
或
git branch --set-upstream-to=origin/remote_branch your_branch
```
- 在master分支上合并新分支
```
git merge your_branch
```
- 合并到远程master分支
```
回到master:git checkout master
先拉：git pull
来到新分支：git checkout your_branch
检查是否与master有冲突：git rebase master
解决冲突后：git add .(只能当前目录)
继续解决冲突：git rebase --continue
把分支推送到远程分支上：git push origin your_branch:master
```

## 实际操作

- git checkout -b development
- 添加一个文件
- git status
- git add --all
- git commit -m "initial on development"
- git push origin development, 就会看到远程多了一个development分支，但是此时在master分支上是看不到刚才新加的内容的，需要合并分支
- git push origin development:master,此时在master分支上可以看到刚刚添加的内容。实际情况中，是在管理后台，由项目管理员主动发起合并。