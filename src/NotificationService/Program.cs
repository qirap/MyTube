using NotificationService.IntegrationEvents.EventHandlers;
using Shared.EventBus.Implementations;
using Shared.EventBus.IntegrationEvents;
using Shared.EventBus.Interfaces;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine(builder.Configuration["Kafka:Url"]);
builder.Services.AddSingleton<IEventBus>(provider =>
{
	return new KafkaEventBus(builder.Configuration["Kafka:Url"]);
});

builder.Services.AddTransient<IIntegrationEventHandler<VideoUploadedIntegrationEvent>,
							  VideoUploadedIntegrationEventHandler>();

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<IEventBus>();

async Task SubscribeToEvents()
{
	try
	{
		Console.WriteLine("�������� �� ������� Kafka...");
		await eventBus.Subscribe<VideoUploadedIntegrationEvent, VideoUploadedIntegrationEventHandler>();
		Console.WriteLine("�������� �������!");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"������ ��������: {ex.Message}");
	}
}

Task.Run(SubscribeToEvents);

app.Run();
