- 右键项目属性生成的时候发布包
- 发布
```
dotnet nuget push ..nupkg -k your_key -s https://api.nuget.org/v3/index.json
```
- 删除
```
dotnet nuget delete package_id version --force-english-output -s https://www.nuget.org/packages
```