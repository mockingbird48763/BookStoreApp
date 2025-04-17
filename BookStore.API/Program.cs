using BookStore.Data;
using BookStore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// �̿�`�J
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

    // �}�ҵ��ѥ\��
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
        // ���ɥؿ��\��
        options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"BookStore API Docs V1");
    });
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureDeleted(); // �C���ҰʲM��
    context.Database.EnsureCreated(); // ���s�ت�
    await SeedData.SeedAsync(app.Services); // ��R��l���
}

app.UseAuthorization();

app.MapControllers();

app.Run();
