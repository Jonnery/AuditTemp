# 审计类库
## 1、目前只实现了Mysql,Oracle,MongoDB,SQLserver
## 2、目前有两种注入方式:
### A委托:
```
 //Oracle委托Action
builder.Services.ConfigureDbContext(action: opt =>
{
   opt.DatabaseType = DatabaseType.Oracle;
   opt.Connect
    ionString = "Data Source=XXXXXXX:1521/XXX;User Id=XXXXXX;Password=XXXX;Pooling=false;Max Pool Size=100;Min Pool Size=10;Load Balancing=true;HA Events=true;Validate Connection=true";
});
```

appsettings.json
~~~

"DatabaseOptions": {
  "DatabaseType": "mysql",
  "Version": "8.0.22",
  "DatabaseName": null,
  "ConnectionString": "Server=localhost; Port=3306;Stmt=; Database=business; Uid=root; Pwd=123456;"
}
~~~
