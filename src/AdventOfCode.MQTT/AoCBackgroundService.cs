using AdventOfCode.Client;
using MQTTnet;
using MQTTnet.Client;

namespace AdventOfCode.MQTT;

public class AoCBackgroundService : BackgroundService
{
    private readonly ILogger<AoCBackgroundService> _logger;
    private readonly MqttFactory _mqttFactory;
    private readonly AoCClient _aoCClient;

    public AoCBackgroundService(MqttFactory mqttFactory, ILogger<AoCBackgroundService> logger, AoCClient aoCClient)
    {
        _mqttFactory = mqttFactory;
        _logger = logger;
        _aoCClient = aoCClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        using var mqttClient = _mqttFactory.CreateMqttClient();
        
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", 1883)
            .WithClientId("aoc-client")
            .WithCredentials("username")
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("samples/temperature/living_room")
            .WithPayload("19.5")
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        await mqttClient.DisconnectAsync();
            
        _logger.LogInformation("MQTT application message was published");
    }
}