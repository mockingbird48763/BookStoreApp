using BookStore.Data;
using BookStore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 依賴注入
builder.Services.AddScoped<IMemberService, MemberService>();
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


    // 讀取當前專案的 XML 註解
    var baseXmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var baseXmlPath = Path.Combine(AppContext.BaseDirectory, baseXmlFile);
    options.IncludeXmlComments(baseXmlPath, includeControllerXmlComments: true);

    // 讀取類別庫的 XML 註解
    var externalAssemblies = new[] { "BookStore.DTO", "BookStore.Models", "BookStore.Data" };
    foreach (var assemblyName in externalAssemblies)
    {
        var xmlFile = $"{assemblyName}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath)) // 確保 XML 檔案存在才載入
        {
            options.IncludeXmlComments(xmlPath);
        }
    }

    // 對 action 進行名稱排序
    options.OrderActionsBy(o => o.RelativePath);

    // 開啟註解功能
    // options.UseOneOfForPolymorphism();
    // options.EnableAnnotations();
});
#endregion

#region DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureDeleted(); // 每次啟動清空
    context.Database.EnsureCreated(); // 重新建表
    await SeedData.SeedAsync(app.Services); // 填充初始資料
}

app.UseAuthorization();

app.MapControllers();

app.Run();
