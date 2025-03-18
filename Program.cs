using Amazon.DynamoDBv2;
using WaterMyGarden.Services;
using WaterMyGarden.Data;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Load AWS configurations from appsettings.json
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

// Register AWS DynamoDB service
builder.Services.AddAWSService<IAmazonDynamoDB>();

builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

// Add services to the container.
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IRecordWateringData, RecordWateringData>();
builder.Services.AddScoped<IAwsSnsServices, AwsSnsServices>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
