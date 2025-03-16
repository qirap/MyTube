using Confluent.Kafka;
using Shared.EventBus.Entities;
using Shared.EventBus.Interfaces;
using System.Text.Json;

namespace Shared.EventBus.Implementations
{
	public class KafkaEventBus : IEventBus
	{
		private readonly string _bootstrapServers;
		private readonly IProducer<Null, string> _producer;
		private readonly IConsumer<Ignore, string> _consumer;

		public KafkaEventBus(string bootstrapServers)
		{
			_bootstrapServers = bootstrapServers;

			var producerConfig = new ProducerConfig { BootstrapServers = _bootstrapServers };
			_producer = new ProducerBuilder<Null, string>(producerConfig).Build();

			var consumerConfig = new ConsumerConfig
			{
				BootstrapServers = _bootstrapServers,
				GroupId = "eventbus-group",
				AutoOffsetReset = AutoOffsetReset.Earliest
			};
			_consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
		}

		public async Task Publish(IntegrationEvent @event)
		{
			var topicName = @event.GetType().Name; // Имя типа события будет названием топика
			var eventMessage = JsonSerializer.Serialize(@event, @event.GetType());

			try
			{
				await _producer.ProduceAsync(topicName, new Message<Null, string> { Value = eventMessage });
				Console.WriteLine($"Published event with Id: {@event.Id} to topic: {topicName}");
			}
			catch (ProduceException<Null, string> ex)
			{
				Console.WriteLine($"Error producing message: {ex.Message}");
			}
		}

		public Task Subscribe<T, TH>()
			where T : IntegrationEvent
			where TH : IIntegrationEventHandler<T>
		{
			return Task.Run(() =>
			{
				var topicName = typeof(T).Name; // Имя типа события будет названием топика
				_consumer.Subscribe(topicName);

				while (true)
				{
					try
					{
						var consumeResult = _consumer.Consume();
						var eventType = typeof(T);
						var @event = (T)JsonSerializer.Deserialize(consumeResult.Message.Value, eventType);
						var handler = Activator.CreateInstance<TH>();
						if (handler != null)
						{
							var handleMethod = handler.GetType().GetMethod("Handle");
							handleMethod?.Invoke(handler, new object[] { @event });
						}
					}
					catch (ConsumeException ex)
					{
						Console.WriteLine($"Error consuming message: {ex.Message}");
					}
				}
			});
		}

		private string SerializeEvent(IntegrationEvent @event)
		{
			return JsonSerializer.Serialize(@event, @event.GetType());
		}

		private T DeserializeEvent<T>(string message) where T : IntegrationEvent
		{
			return JsonSerializer.Deserialize<T>(message);
		}
	}
}
