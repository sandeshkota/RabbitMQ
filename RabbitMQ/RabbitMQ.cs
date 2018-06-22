using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQ
{
    public class RabbitMQ
    {
        #region Constructor & Private members

        private readonly string _hostName = "server.in.server.com";
        private readonly string _userName = "userName";
        private readonly string _password = "passWord";
        private readonly string _virtualHost = "hostName";
        private IModel _model;

        private readonly string _exchangeName = "Sample.MyExchange";
        private readonly string _queueName = "Sample.MyQueue";
        private readonly string _secondQueueName = "Sample.MySecondQueue";
        private readonly string _thirdQueueName = "Sample.MyThirdQueue";
        private readonly string _fourthQueueName = "Sample.MyFourthQueue";

        private readonly string _deadLetterExchangeName = "Sample.DeadLetterExchange";
        private readonly string _deadLetterQueueName = "Sample.DeadLetterQueue";

        private readonly string _routingFailureExchangeName = "Sample.RoutingFailureExchange";
        private readonly string _routingFailureQueueName = "Sample.RoutingFailureQueue";

        private readonly string _holdingExchangeName = "Sample.HoldingExchange";
        private readonly string _holdingQueueName = "Sample.HoldingQueue";

        private Dictionary<string, object> _arguements = null;

        private string _exchangeType = ExchangeType.Direct;
        private readonly bool _isDurable = false;
        private readonly int _port = 0;

        public RabbitMQ()
        {
        }

        public RabbitMQ(string userName, string password, string hostName, string virtualHost)
        {
            _userName = userName;
            _password = password;
            _hostName = hostName;
            _virtualHost = virtualHost;
        }

        #endregion

        #region Private Functions
        private void DeleteExistingExchangesAndQueues()
        {
            // Exchanges
            _model.ExchangeDelete(_exchangeName);
            _model.ExchangeDelete(_deadLetterExchangeName);
            _model.ExchangeDelete(_routingFailureExchangeName);
            _model.ExchangeDelete(_deadLetterExchangeName);

            // Queues
            _model.QueueDelete(_queueName);
            _model.QueueDelete(_secondQueueName);
            _model.QueueDelete(_thirdQueueName);
            _model.QueueDelete(_fourthQueueName);
            _model.QueueDelete(_deadLetterQueueName);
            _model.QueueDelete(_routingFailureQueueName);
            _model.QueueDelete(_holdingQueueName);
        }

        private void DeclareExchangeAndQueue()
        {
            DeclareExchange(_exchangeName);
            DeclareQueue(_queueName);
            BindQueueToExchange(_queueName, _exchangeName);
        }

        private void DeclareExchange(string exchangeName)
            => _model.ExchangeDeclare(exchangeName, _exchangeType);

        private void DeclareQueue(string queueName, Dictionary<string, object> parameters = null)
            => _model.QueueDeclare(queueName, _isDurable, false, false, parameters);

        private void BindQueueToExchange(string queueName, string exchangeName, string routingKey = "")
            => _model.QueueBind(queueName, exchangeName, routingKey, _arguements);

        #endregion

        #region Public Functions

        public void SetUp()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };
            if (!string.IsNullOrEmpty(_virtualHost)) connectionFactory.VirtualHost = _virtualHost;
            if (_port > 0) connectionFactory.Port = _port;

            var connection = connectionFactory.CreateConnection();
            _model = connection.CreateModel();

            DeleteExistingExchangesAndQueues();
        }



        public void CreateSetupForOneWayMessaging() => DeclareExchangeAndQueue();

        public void CreateSetupForWorkerQueues() => DeclareExchangeAndQueue();

        public void CreateSetupForPublishSubscribe()
        {
            _exchangeType = ExchangeType.Fanout;
            DeclareExchange(_exchangeName);
            DeclareQueue(_queueName);
            BindQueueToExchange(_queueName, _exchangeName);
            DeclareQueue(_secondQueueName);
            BindQueueToExchange(_secondQueueName, _exchangeName);
        }

        public void CreateSetupForRemoteProcedureCall() => DeclareExchangeAndQueue();

        public void CreateSetupForRouting()
        {
            DeclareExchange(_exchangeName);

            DeclareQueue(_queueName);
            BindQueueToExchange(_queueName, _exchangeName, "1");
            DeclareQueue(_secondQueueName);
            BindQueueToExchange(_secondQueueName, _exchangeName, "2");
        }

        public void CreateSetupForTopics()
        {
            _exchangeType = ExchangeType.Topic;
            DeclareExchange(_exchangeName);

            DeclareQueue(_queueName);
            BindQueueToExchange(_queueName, _exchangeName, "*.high.*");
            DeclareQueue(_secondQueueName);
            BindQueueToExchange(_secondQueueName, _exchangeName, "*.*.cupboard");
            DeclareQueue(_thirdQueueName);
            BindQueueToExchange(_thirdQueueName, _exchangeName, "corporate.#");
            BindQueueToExchange(_thirdQueueName, _exchangeName, "*.medium.*");
        }

        public void CreateSetupForHeaders()
        {
            _exchangeType = ExchangeType.Headers;
            _arguements = new Dictionary<string, object>();
            DeclareExchange(_exchangeName);

            DeclareQueue(_queueName);
            _arguements.Add("x-match", "all");
            _arguements.Add("material", "wood");
            _arguements.Add("customerType", "b2b");
            BindQueueToExchange(_queueName, _exchangeName);
            _arguements.Clear();

            DeclareQueue(_secondQueueName);
            _arguements.Add("x-match", "all");
            _arguements.Add("material", "metal");
            _arguements.Add("customerType", "b2c");
            BindQueueToExchange(_secondQueueName, _exchangeName);
            _arguements.Clear();

            DeclareQueue(_thirdQueueName);
            _arguements.Add("x-match", "any");
            _arguements.Add("material", "wood");
            _arguements.Add("customerType", "b2b");
            BindQueueToExchange(_thirdQueueName, _exchangeName);
            _arguements.Clear();

            DeclareQueue(_fourthQueueName);
            _arguements.Add("x-match", "any");
            _arguements.Add("material", "metal");
            _arguements.Add("customerType", "b2c");
            BindQueueToExchange(_fourthQueueName, _exchangeName);
            _arguements.Clear();

        }

        public void CreateSetupForScatterGather()
        {
            _exchangeType = ExchangeType.Topic;
            DeclareExchange(_exchangeName);

            DeclareQueue(_queueName);
            BindQueueToExchange(_queueName, _exchangeName, "1");
            BindQueueToExchange(_queueName, _exchangeName, "4");

            DeclareQueue(_secondQueueName);
            BindQueueToExchange(_secondQueueName, _exchangeName, "2");
            BindQueueToExchange(_secondQueueName, _exchangeName, "4");
            BindQueueToExchange(_secondQueueName, _exchangeName, "6");

            DeclareQueue(_thirdQueueName);
            BindQueueToExchange(_thirdQueueName, _exchangeName, "3");
            BindQueueToExchange(_thirdQueueName, _exchangeName, "4");
            BindQueueToExchange(_thirdQueueName, _exchangeName, "6");
        }

        public void CreateSetupForSerialization() => DeclareExchangeAndQueue();

        public void CreateSetupForMessageType() => DeclareExchangeAndQueue();

        public void CreateSetupForLargeBufferedMessage() => DeclareExchangeAndQueue();

        public void CreateSetupForLargeChunkedMessage() => DeclareExchangeAndQueue();

        public void CreateSetupForBasicRetry() => DeclareExchangeAndQueue();

        public void CreateSetupForAdvancedRetry() => DeclareExchangeAndQueue();

        public void CreateSetupForDeadLetterQueue()
        {
            _exchangeType = ExchangeType.Fanout;
            DeclareExchange(_deadLetterExchangeName);
            DeclareQueue(_deadLetterQueueName);
            BindQueueToExchange(_deadLetterQueueName, _deadLetterExchangeName);

            _exchangeType = ExchangeType.Direct;
            DeclareExchange(_exchangeName);
            var parameters = new Dictionary<string, object> { { "x-dead-letter-exchange", _deadLetterExchangeName } };
            DeclareQueue(_queueName, parameters);
            BindQueueToExchange(_queueName, _exchangeName);
        }

        public void CreateSetupForRoutingFailure()
        {
            _exchangeType = ExchangeType.Fanout;
            DeclareExchange(_routingFailureExchangeName);
            DeclareQueue(_routingFailureQueueName);
            BindQueueToExchange(_routingFailureQueueName, _routingFailureExchangeName);

            _exchangeType = ExchangeType.Topic;
            var parameters = new Dictionary<string, object> { { "alternate-exchange", _routingFailureExchangeName } };
            _model.ExchangeDeclare(_exchangeName, _exchangeType, false, false, parameters);
            DeclareQueue(_queueName);
            BindQueueToExchange(_queueName, _exchangeName, "Apples");
            BindQueueToExchange(_queueName, _exchangeName, "Oranges");
        }

        public void CreateSetupForScheduledDelivery()
        {
            _exchangeType = ExchangeType.Fanout;
            DeclareExchange(_exchangeName);
            DeclareQueue(_queueName);
            BindQueueToExchange(_queueName, _exchangeName);

            _exchangeType = ExchangeType.Direct;
            DeclareExchange(_holdingExchangeName);
            var parameters = new Dictionary<string, object> { { "x-dead-letter-exchange", _exchangeName } };
            DeclareQueue(_holdingQueueName, parameters);
            BindQueueToExchange(_holdingQueueName, _holdingExchangeName);
        }

        #endregion


    }
}
