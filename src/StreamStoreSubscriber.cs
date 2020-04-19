using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using LiquidProjections.Abstractions;
using Newtonsoft.Json;
using SqlStreamStore.Demo.Events.Account;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo
{
    public class StreamStoreSubscriber : IDisposable
    {
        private readonly IStreamStore _streamStore;
        private readonly StreamId _streamId;
        private readonly IEventSerializer _eventSerializer;

        public StreamStoreSubscriber(IStreamStore streamStore, StreamId streamId, IEventSerializer eventSerializer)
        {
            _streamStore = streamStore;
            _streamId = streamId;
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
        }

        public IDisposable Subscribe(long? lastProcessedCheckpoint, Subscriber subscriber, string subscriptionId)
        {
            var subscription = new StreamStoreSubscription(lastProcessedCheckpoint?? 0, subscriber, _eventSerializer);
                subscription.Subscribe(_streamStore, _streamId);
            return subscription;
        }

        public void Dispose()
        {
            //_streamStore?.Dispose();
        }
    }
}