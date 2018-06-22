using System;
using System.Collections;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DeadLetterQueueProcessor
{
    public class DeadLetterConsumer
    {
        private readonly string _hostName = "server.in.server.com";
        private readonly string _userName = "userName";
        private readonly string _password = "passWord";
        private readonly string _virtualHost = "hostName";

        private IModel _model;
        private const string _exchangeName = "CashAccount.DeadLetterExchange";
        private const string _queueName = "CashAccount.DeadLetterQueue";
        private const string _resubmitExchangeName = "CashAccount.MyExchange";
        private readonly int _port = 0;
        public bool Enabled { get; set; }

        public DeadLetterConsumer()
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
                var deliveryArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                var message = Encoding.Default.GetString(deliveryArgs.Body);

                Console.WriteLine("Message Received: {0}", message);
                Console.WriteLine();
                Console.WriteLine("Changing message to 1");
                message = "1";

                var properties = _model.CreateBasicProperties();
                properties.Persistent = true;
                byte[] messageBuffer = Encoding.Default.GetBytes(message);

                // Resend message to original Queue
                //var resubmitExchange = GetExchange(deliveryArgs);
                _model.BasicPublish(_resubmitExchangeName, "", properties, messageBuffer);

                // Acknowledge Dead Letter Queue
                _model.BasicAck(deliveryArgs.DeliveryTag, false);
            }
        }

        private static string GetExchange(BasicDeliverEventArgs deliveryArgs)
        {
            var headers = deliveryArgs.BasicProperties.Headers;

            var header = headers?["x-death"];

            var xDeathHeader = header as ArrayList;
            if (xDeathHeader == null || xDeathHeader.Count < 1) return string.Empty;

            var properties = xDeathHeader[0] as Hashtable;
            if (properties == null || properties.Count < 1) return string.Empty;

            if (properties.Contains("exchange"))
            {
                var queueBytes = properties["exchange"] as byte[];
                return Encoding.Default.GetString(queueBytes);
            }

            return string.Empty;
        }
    }
}
