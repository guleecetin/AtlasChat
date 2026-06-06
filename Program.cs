using AtlasChat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<IAIService, OllamaAIService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();