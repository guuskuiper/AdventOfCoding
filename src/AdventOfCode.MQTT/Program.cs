using System.Reflection;
using System.Text;
using AdventOfCode.Client;
using AdventOfCode.MQTT;
using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(x =>
{
    x.ListenAnyIP(1883, options => options.UseMqtt());
    x.ListenAnyIP(5087);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
string session = await File.ReadAllTextAsync(Path.Combine(dir, "SESSION"));
builder.Services.AddSingleton(new AoCClient(session));

builder.Services.AddHostedService<AoCBackgroundService>();
builder.Services
    .AddHostedMqttServer(mqttServer => mqttServer.WithDefaultEndpoint())
    .AddMqttConnectionHandler()
    .AddConnections();

var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.MapMqtt("/mqtt");

app.UseMqttServer(server =>
{
    server.ClientConnectedAsync += eventArgs =>
    {
        app.Logger.LogInformation("Client connected: {EventArgsUserName} - {EventArgsClientId}", eventArgs.UserName, eventArgs.ClientId);
        return Task.CompletedTask;
    }; 
    server.StartedAsync += args =>
    {
        app.Logger.LogInformation("Server started");
        return Task.CompletedTask;
    };
    server.InterceptingPublishAsync += eventArgs =>
    {
        string message = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment.Array ?? Array.Empty<byte>());
        app.Logger.LogInformation("Received: {EventArgsClientId}, {ApplicationMessageTopic}: {Message}", eventArgs.ClientId, eventArgs.ApplicationMessage.Topic, message);
        return Task.CompletedTask;
    };
});

app.Run();
