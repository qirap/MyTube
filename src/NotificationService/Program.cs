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
		Console.WriteLine("Подписка на события Kafka...");
		await eventBus.Subscribe<VideoUploadedIntegrationEvent, VideoUploadedIntegrationEventHandler>();
		Console.WriteLine("Подписка успешна!");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Ошибка подписки: {ex.Message}");
	}
}

Task.Run(SubscribeToEvents);

app.Run();
