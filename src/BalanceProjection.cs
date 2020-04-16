using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiquidProjections;
using Newtonsoft.Json;
using SqlStreamStore.Demo.Events.Account;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo
{

    public class AccountInfo
    {
        public IList<AccountEvent> Transactions { get; } = new List<AccountEvent>();
        public Balance Balance { get; } = new Balance(0, DateTime.Now);


        public void Add(AccountEvent transaction)
        {
            Transactions.Add(transaction);
            if (transaction is AmountDeposited deposit)
            {
                Balance.Add(deposit.Amount);
            }
            else if (transaction is AmountWithdrawn withdrawn)
            {
                Balance.Subtract(withdrawn.Amount);
            }
        }

    }


    public class BalanceProjection
    {
        private readonly IEventMap<AccountInfo> _map;
        private readonly IEventSerializer _eventSerializer;
        private readonly AccountInfo _accountInfo;
        public BalanceProjection(IStreamStore streamStore, StreamId streamId, IEventSerializer eventSerializer, AccountInfo info)
        {
            _eventSerializer = eventSerializer;
            _accountInfo = info;

            var mapBuilder = new EventMapBuilder<AccountInfo>();
            mapBuilder.Map<AmountDeposited>().As((deposited, info) => { info.Add(deposited); });
            mapBuilder.Map<AmountWithdrawn>().As((withdrawn, info) => { info.Add(withdrawn); });

            _map = mapBuilder.Build(new ProjectorMap<AccountInfo>()
            {
                Custom = (context, projector) => projector()
            });

            streamStore.SubscribeToStream(streamId, null, StreamMessageReceived);
        }

        private async Task<object> DeserializeJsonEvent(StreamMessage streamMessage,
            CancellationToken cancellationToken)
        {
            var json = await streamMessage.GetJsonData(cancellationToken);
            return _eventSerializer.DeserializeEvent(streamMessage.Type, json);

        }

        private async Task StreamMessageReceived(IStreamSubscription subscription, StreamMessage streamMessage,
            CancellationToken cancellationToken)
        {
            var @event = await DeserializeJsonEvent(streamMessage, cancellationToken);
            await _map.Handle(@event, _accountInfo);
        }
    } 
}