using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Client
{
    class RabbitSender
    {
        private readonly string _hostName = "server.in.server.com";
        private readonly string _userName = "userName";
        private readonly string _password = "passWord";
        private readonly string _virtualHost = "hostName";
        private IModel _model;
        private string _responseQueue;
        private QueueingBasicConsumer _consumer;
        private const string _exchangeName = "CashAccount.MyExchange";
        private const string _queueName = "CashAccount.MyQueue";
        private const string _routingKey = "MyRouting";
        private readonly int _port = 0;

        public RabbitSender()
        {
            DisplaySettings();
            SetUpRabbitMq();
        }

        private void DisplaySettings()
        {
            Console.WriteLine("ExchangeName: {0}", _exchangeName);
            Console.WriteLine("QueueName: {0}", _queueName);
        }

        private void SetUpRabbitMq()
        {
            var connectionFactory = new ConnectionFactory { HostName = _hostName, UserName = _userName, Password = _password };
            if (string.IsNullOrEmpty(_virtualHost) == false) connectionFactory.VirtualHost = _virtualHost;
            if (_port > 0) connectionFactory.Port = _port;

            var connection = connectionFactory.CreateConnection();
            _model = connection.CreateModel();

            // Create dynamic response Queue
            _responseQueue = _model.QueueDeclare($"CashAccount.{Guid.NewGuid()}").QueueName;
            _model.QueueBind(_responseQueue, _exchangeName, _routingKey, null);
            _consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(_responseQueue, true, _consumer);
        }


        public string Send(string message, TimeSpan timeout)
        {
            var correlationtoken = Guid.NewGuid().ToString();

            // setup Properties
            var properties = _model.CreateBasicProperties();
            properties.ReplyTo = _responseQueue;
            properties.CorrelationId = correlationtoken;

            // serialize
            var messageBuffer = Encoding.Default.GetBytes(message);

            //send
            var timeoutAt = DateTime.Now + timeout;
            _model.BasicPublish(_exchangeName, _queueName, properties, messageBuffer);

            // Wait for response
            while (DateTime.Now <= timeoutAt)
            {
                var deliveryArgs = (BasicDeliverEventArgs)_consumer.Queue.Dequeue();

                if (deliveryArgs.BasicProperties != null && deliveryArgs.BasicProperties.CorrelationId == correlationtoken)
                {
                    var response = Encoding.Default.GetString(deliveryArgs.Body);
                    return response;
                }
            }
            throw new TimeoutException("The response was not returned after the timeout");
        }
    }
}