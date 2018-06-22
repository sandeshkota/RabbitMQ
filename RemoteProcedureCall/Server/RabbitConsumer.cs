using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace Server
{
    public class RabbitConsumer
    {
        private readonly string _hostName = "server.in.server.com";
        private readonly string _userName = "userName";
        private readonly string _password = "passWord";
        private readonly string _virtualHost = "hostName";
        private IModel _model;
        private Subscription _subscription;
        private const string _exchangeName = "CashAccount.MyExchange";
        private const string _queueName = "CashAccount.MyQueue";
        private const string _routingKey = "MyRouting";
        private const bool _isDurable = false;
        private readonly int _port = 0;
        public bool Enabled { get; set; }

        public RabbitConsumer()
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
            _model.BasicQos(0, 1, false);
        }

        public void Start()
        {
            var consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(_queueName, false, consumer);


            while (Enabled)
            {
                // Get Next Mesasge
                var deliveryArgs = (BasicDeliverEventArgs) consumer.Queue.Dequeue();

                var message = Encoding.Default.GetString(deliveryArgs.Body);
                Console.WriteLine("received message: {0}", message);
                var response = $"Processed message - {message} : Response is success";

                // Send response
                var replyProperties = _model.CreateBasicProperties();
                replyProperties.CorrelationId = deliveryArgs.BasicProperties.CorrelationId;
                var messageBuffer = Encoding.Default.GetBytes(response);
                _model.BasicPublish(_exchangeName, _routingKey, replyProperties, messageBuffer);

                // Acknowledge message is processed
                _model.BasicAck(deliveryArgs.DeliveryTag, false);
            }
        }

       
    }
}
