using Audit.Realization.Configure;
using Audit.Realization.EntityFrameworkCore;
using Audit.Realization.Options;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = builder.Configuration;

//mysql
/*var op = new DatabaseOption()
{
    DatabaseType = DatabaseType.MySql,
    ConnectionString = "Server=localhost; Port=3306;Stmt=; Database=business; Uid=root; Pwd=123456;",
    //DatabaseName = "AuditLogTest",
    Version = "8.0.22"
};*/
/*var op = new DatabaseOption()
{
    DatabaseType = DatabaseType.MongoDb,
    DatabaseName = "AuditLogTest",
};*/
// Oracle委托Action
//builder.Services.ConfigureDbContext(action: opt =>
//{
//    opt.DatabaseType = DatabaseType.Oracle;
//    opt.Connect
//    ionString = "Data Source=192.168.82.11:1521/inner1;User Id=SMART_PLATFORM_V01;Password=SMARTS;Pooling=false;Max Pool Size=100;Min Pool Size=10;Load Balancing=true;HA Events=true;Validate Connection=true";
//});
//选项模式
builder.Services.ConfigureMyDbContext(config);
//builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
