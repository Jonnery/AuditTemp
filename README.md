
# 审计类库
### 1、目前只实现了Mysql,Oracle,MongoDB,SQLserver
### 2、目前有两种注入方式:
统一在配置文件加上数据库信息
appsettings.json
~~~

"DatabaseOptions": {
  "DatabaseType": "mysql",
  "Version": "8.0.22",
  "DatabaseName": null,
  "ConnectionString": "Server=localhost; Port=3306;Stmt=; Database=business; Uid=root; Pwd=123456;"
}
~~~
- A委托:
```
 //Oracle委托Action
builder.Services.ConfigureDbContext(action: opt =>
{
   opt.DatabaseType = DatabaseType.Oracle;
   opt.Connect
    ionString = "Data Source=XXXXXXX:1521/XXX;User Id=XXXXXX;Password=XXXX;Pooling=false;Max Pool Size=100;Min Pool Size=10;Load Balancing=true;HA Events=true;Validate Connection=true";
});
```

- 选项模式
//选项模式
builder.Services.ConfigureMyDbContext(config);

在控制器加上标签: [Audited]
若要排除某个接口不做审计,则在相应的方法加上标签:DisableAuditing
