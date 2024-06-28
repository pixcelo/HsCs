// 名前付きCORSポリシー
var sampleSpecificOrigins = "sampleSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// https://learn.microsoft.com/ja-jp/aspnet/core/security/cors?view=aspnetcore-8.0
// CORSポリシーの設定
builder.Services.AddCors(options =>
{
    options.AddPolicy(sampleSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("https://localhost:7143");
        });
});

// Add services to the container.

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

// 名前付きCORSポリシーを指定
app.UseCors(sampleSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
