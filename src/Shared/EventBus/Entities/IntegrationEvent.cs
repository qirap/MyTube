﻿using System.Text.Json.Serialization;

namespace Shared.EventBus.Entities
{
	public abstract class IntegrationEvent
	{
		public IntegrationEvent()
		{
			Id = Guid.NewGuid();
			CreationDate = DateTime.UtcNow;
		}

		[JsonInclude]
		public Guid Id { get; set; }

		[JsonInclude]
		public DateTime CreationDate { get; set; }
	}
}
