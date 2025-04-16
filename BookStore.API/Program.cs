using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookStore API Docs",
        Version = "v1",
        Description = "一個簡單的 API 用於管理書店的庫存、顧客與訂單"
    });

    // xml 文檔絕對路徑
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);    // true 顯示 controller 註解
    options.IncludeXmlComments(xmlPath, true);
    // 對 action 進行名稱排序
    options.OrderActionsBy(o => o.RelativePath);
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // 文檔目錄功能
        options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"BookStore API Docs V1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
