using Amazon.SimpleEmail;
using Flashcards.Configuration;
using Flashcards.Controllers.Frontpage;
using Flashcards.Controllers.KeyCrmReports;
using Flurl.Http.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>();
builder.Services.AddAWSService<IAmazonSimpleEmailService>();

var jsonBinConfig = new JsonBinConfig();
builder.Configuration.Bind("JsonBin", jsonBinConfig);
builder.Services.AddSingleton(jsonBinConfig);
builder.Services.AddScoped<JsonBinConnector>();

var keyCrmConfig = new KeyCrmConfig();
builder.Configuration.Bind("KeyCrm", keyCrmConfig);
builder.Services.AddSingleton(keyCrmConfig);
builder.Services.AddScoped<KeyCrmConnector>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

//app.UseHttpsRedirection();
app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();