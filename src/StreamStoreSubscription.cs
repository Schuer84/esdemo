using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiquidProjections;
using LiquidProjections.Abstractions;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo
{
    public class StreamStoreSubscription : IDisposable
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly Subscriber _subscriber;
        private readonly long _lastCheckpoint;

        public StreamStoreSubscription(long lastCheckpoint, Subscriber subscriber, IEventSerializer eventSerializer)
        {
            _lastCheckpoint = lastCheckpoint;
            _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
        }

        protected IStreamSubscription StreamSubscription { get; set; }

        public void Subscribe(IStreamStore streamStore, StreamId streamId)
        {
            StreamSubscription = streamStore.SubscribeToStream(streamId, null, StreamMessageReceived);
        }


        private async Task<object> DeserializeJsonEvent(StreamMessage streamMessage,
            CancellationToken cancellationToken)
        {
            var json = await streamMessage.GetJsonData(cancellationToken);
            return _eventSerializer.DeserializeEvent(streamMessage.Type, json);

        }
        public async Task StreamMessageReceived(IStreamSubscription subscription, StreamMessage streamMessage,
            CancellationToken cancellationToken)
        {
            if (streamMessage.Position >= _lastCheckpoint)
            {
                var @event = await DeserializeJsonEvent(streamMessage, cancellationToken);
                var transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Id = subscription.Name,
                        Checkpoint = streamMessage.Position,
                        StreamId = subscription.StreamId,
                        Events = new List<EventEnvelope>()
                        {
                            new EventEnvelope()
                            {
                                Body = @event
                            }
                        }
                    }
                };

                await _subscriber.HandleTransactions(transactions, new SubscriptionInfo()
                {
                    Id = "transaction",
                    CancellationToken = cancellationToken,
                    Subscription = this
                });
            }
        }

        public void Dispose()
        {
            StreamSubscription?.Dispose();
        }
    }
}