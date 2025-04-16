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
        Description = "�@��²�檺 API �Ω�޲z�ѩ����w�s�B�U�ȻP�q��"
    });

    // xml ���ɵ�����|
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);    // true ��� controller ����
    options.IncludeXmlComments(xmlPath, true);
    // �� action �i��W�ٱƧ�
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
        // ���ɥؿ��\��
        options.SwaggerEndpoint($"/swagger/v1/swagger.json", $"BookStore API Docs V1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
