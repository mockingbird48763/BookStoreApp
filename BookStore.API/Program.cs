using BookStore.Core.Settings;
using BookStore.Data;
using BookStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 依賴注入
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IAuthService, AuthService>();
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

    #region Configure JWT Token Authorization
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter the token in the format: Bearer xxxxxx",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    #endregion
});
#endregion

#region DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region JwtSettings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
#endregion

# region 設定 JWT 認證
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // 是否要求 HTTPS (視情況設定)
        options.SaveToken = true; // 儲存 Token
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>(); // 使用 DI 容器來取得 JwtSettings 配置
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // 系統預設容許 5 分鐘，需要手動調整
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });
# endregion

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

app.UseAuthentication();  // 確保啟用認證
app.UseAuthorization();   // 確保啟用授權
app.UseStaticFiles(); // 確保已經啟用靜態檔案服務

app.MapControllers();

app.Run();
