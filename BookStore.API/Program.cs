using BookStore.API.Middleware;
using BookStore.Core.Settings;
using BookStore.Core.Strategies;
using BookStore.Data;
using BookStore.DTO.Request;
using BookStore.DTO.Validators;
using BookStore.Services;
using BookStore.Services.FileStorageStrategies;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using BookStore.DTO.Profiles;

var builder = WebApplication.CreateBuilder(args);

#region DependencyInjection
builder.Services.AddScoped<IMembersService, MembersService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBooksService, BooksService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IImageStorageStrategy, LocalImageStorageStrategy>();
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserInfoContext, UserInfoContext>();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IImageStorageStrategy, CloudImageStorageStrategy>();
}
builder.Services.AddControllers();
#endregion

#region FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(BookProfile));
#endregion

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookStore API Docs",
        Version = "v1",
        Description = "�@��²�檺 API �Ω�޲z�ѩ����w�s�B�U�ȻP�q��"
    });

    // Ū����e�M�ת� XML ����
    var baseXmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var baseXmlPath = Path.Combine(AppContext.BaseDirectory, baseXmlFile);
    options.IncludeXmlComments(baseXmlPath, includeControllerXmlComments: true);

    // Ū�����O�w�� XML ����
    var externalAssemblies = new[] { "BookStore.DTO", "BookStore.Models", "BookStore.Data" };
    foreach (var assemblyName in externalAssemblies)
    {
        var xmlFile = $"{assemblyName}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath)) // �T�O XML �ɮצs�b�~���J
        {
            options.IncludeXmlComments(xmlPath);
        }
    }

    // �� action �i��W�ٱƧ�
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

# region �]�w JWT �{��
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // �O�_�n�D HTTPS (�����p�]�w)
        options.SaveToken = true; // �x�s Token
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new InvalidOperationException("JWT settings not configured."); // �ϥ� DI �e���Ө��o JwtSettings �t�m
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // �t�ιw�]�e�\ 5 �����A�ݭn��ʽվ�
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
        // ���ɥؿ��\��
        options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"BookStore API Docs V1");
    });

    // dotnet run --seed
    if (args.Contains("--seed"))
    {
        await InitializeDatabaseAsync(app.Services);
        Console.WriteLine("Database initialization complete.");
    }
}


app.UseAuthentication();  // �T�O�ҥλ{��
app.UseAuthorization();   // �T�O�ҥα��v
app.UseStaticFiles(); // �T�O�w�g�ҥ��R�A�ɮתA��
app.UseMiddleware<ExceptionMiddleware>(); // �������`�B�z��

app.MapControllers();

app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    context.Database.EnsureDeleted();  // �C���ҰʲM��
    context.Database.EnsureCreated();  // ���s�ت�
    await SeedData.SeedAsync(services); // ��R��l���
}