using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<NodeRegistryService>();
builder.Services.AddSingleton<MetricsService>();
builder.Services.AddSingleton<LoadBalancerService>();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.MapControllers();
app.Run();