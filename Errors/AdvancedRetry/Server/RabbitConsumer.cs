using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Server
{
    public class RabbitConsumer
    {
        private readonly string _hostName = "server.in.server.com";
        private readonly string _userName = "userName";
        private readonly string _password = "passWord";
        private readonly string _virtualHost = "hostName";
        private const string RetryHeader = "RETRY-COUNT";

        private IModel _model;
        private const string _exchangeName = "CashAccount.MyExchange";
        private const string _queueName = "CashAccount.MyQueue";
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
                var deliveryArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                var message = Encoding.Default.GetString(deliveryArgs.Body);

                Console.WriteLine("Message Received: {0}", message);

                if (message == "1")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Acknowledging successful processing of message");
                    Console.ForegroundColor = ConsoleColor.White;

                    _model.BasicAck(deliveryArgs.DeliveryTag, false);
                }
                else if (message == "2")
                {
                    var attempts = GetRetryAttempts(deliveryArgs.BasicProperties);
                    if (attempts < 3)
                    {
                        Console.WriteLine("Message is 2 so rejecting and requeuing message");
                        Console.WriteLine($"Attempts made: {attempts++}");

                        var properties = _model.CreateBasicProperties();
                        properties.Headers = CopyMessageHeaders(deliveryArgs.BasicProperties.Headers);
                        SetRetryAttempts(properties, attempts);

                        _model.BasicPublish(deliveryArgs.Exchange, deliveryArgs.RoutingKey, properties,
                            deliveryArgs.Body);

                        _model.BasicAck(deliveryArgs.DeliveryTag, false);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Message Rejected for Retry");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Message is 2 but already made {attempts} attempts");
                        Console.ForegroundColor = ConsoleColor.White;

                        _model.BasicReject(deliveryArgs.DeliveryTag, false);
                    }

                }
                else if (message == "3")
                {
                    var attempts = GetRetryAttempts(deliveryArgs.BasicProperties);
                    if (attempts < 2)
                    {
                        Console.WriteLine("Message is 3 so rejecting and requeuing message");
                        Console.WriteLine($"Attempts made: {attempts++}");

                        var properties = _model.CreateBasicProperties();
                        properties.Headers = CopyMessageHeaders(deliveryArgs.BasicProperties.Headers);
                        SetRetryAttempts(properties, attempts);

                        _model.BasicPublish(deliveryArgs.Exchange, deliveryArgs.RoutingKey, properties,
                            deliveryArgs.Body);

                        _model.BasicAck(deliveryArgs.DeliveryTag, false);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Message Rejected for Retry");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(
                            $"Message is 3 and {attempts} attempts have been made so this message is accepted");
                        Console.ForegroundColor = ConsoleColor.White;

                        _model.BasicAck(deliveryArgs.DeliveryTag, false);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Message is >3 so rejecting and not requeuing the message");
                    Console.ForegroundColor = ConsoleColor.White;

                    _model.BasicReject(deliveryArgs.DeliveryTag, false);
                }

            }
        }

        private void SetRetryAttempts(IBasicProperties properties, int attempts)
        {
            if(properties.Headers.ContainsKey(RetryHeader))
                properties.Headers[RetryHeader] = attempts;
            else
                properties.Headers.Add(RetryHeader, attempts);
        }



        private IDictionary<string, object> CopyMessageHeaders(IDictionary<string, object> headers)
        {
            return headers == null ? new Dictionary<string, object>() : 
                headers.ToDictionary(header => header.Key, header => header.Value);
        }

    

        private int GetRetryAttempts(IBasicProperties basicProperties) 
            => basicProperties.Headers != null && basicProperties.Headers.ContainsKey(RetryHeader) 
                                                    && basicProperties.Headers[RetryHeader] != null ?
            Convert.ToInt32(basicProperties.Headers[RetryHeader]) : 0;
    }
}
