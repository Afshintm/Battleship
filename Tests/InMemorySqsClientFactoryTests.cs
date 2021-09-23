using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using SampleTests.Core.Abstracts;
using SampleTests.Domain.SeedWork;
using Xunit;

namespace SampleTests.Components.Tests
{
    public class InMemorySqsClientFactoryTests
    {
        [Fact]
        public async Task Should_Process_Event_When_Using_InMemoryWebServer()
        {
            var notificationHandler = new InMemoryEventHandler();
            IQueueDomainEventsService queueDomainEventsServicePoint = null;

            var server = await InMemoryWebServer.CreateServerAsync(
                services =>
                {
                    services.AddTransient<INotificationHandler<InMemoryEvent>>(_ => notificationHandler);

                    var serviceProvider = services.BuildServiceProvider();
                    queueDomainEventsServicePoint = serviceProvider.GetRequiredService<IQueueDomainEventsService>();
                }
            );

            using (server)
            {
                // act
                var theEvent = new InMemoryEvent(Guid.NewGuid());
                await queueDomainEventsServicePoint.Publish(theEvent);

                // assert
                var processingPolicy =
                    Policy.Handle<Exception>().WaitAndRetry(10, retryAttempt => retryAttempt * TimeSpan.FromSeconds(1));
                processingPolicy.Execute(
                    () => notificationHandler.CapturedEvents.Should().Contain(e => e.EventId == theEvent.EventId, "the expected event was not observed")
                );
            }
        }

        public sealed class InMemoryEvent : IDomainEvent
        {
            public InMemoryEvent(Guid eventId)
            {
                EventId = eventId;
            }

            public Guid EventId { get; set; }
            public Guid CorrelationId { get; private set; }
            public bool ExecuteForTheLastTime { get; private set; }
            public int PreviousExecutionCount { get; private set; }

            public void SetCorrelationId(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public void SetExecuteForTheLastTime(bool executeForTheLastTime)
            {
                ExecuteForTheLastTime = executeForTheLastTime;
            }

            public void SetPreviousExecutionCount(int approximateReceiveCount)
            {
                PreviousExecutionCount = approximateReceiveCount;
            }
        }

        public sealed class InMemoryEventHandler : INotificationHandler<InMemoryEvent>
        {
            public Task Handle(InMemoryEvent notification, CancellationToken cancellationToken)
            {
                CapturedEvents.Add(notification);
                return Task.CompletedTask;
            }

            public List<InMemoryEvent> CapturedEvents { get; } = new List<InMemoryEvent>();
        }
    }
}
