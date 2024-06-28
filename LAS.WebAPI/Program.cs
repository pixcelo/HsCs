// ���O�t��CORS�|���V�[
var sampleSpecificOrigins = "sampleSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// https://learn.microsoft.com/ja-jp/aspnet/core/security/cors?view=aspnetcore-8.0
// CORS�|���V�[�̐ݒ�
builder.Services.AddCors(options =>
{
    options.AddPolicy(sampleSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("https://localhost:7143")
                .AllowAnyMethod()
                .AllowAnyHeader();
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

// ���O�t��CORS�|���V�[���w��
app.UseCors(sampleSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
