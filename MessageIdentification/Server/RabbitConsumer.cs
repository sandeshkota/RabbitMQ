using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Contract;
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

                var messageType = deliveryArgs.BasicProperties.Type;
                var contentType = deliveryArgs.BasicProperties.ContentType;
                var messageString = GetMessageString(deliveryArgs.Body, contentType);
                Console.WriteLine($"Message Type = {messageType}");
                Console.WriteLine($"Message Content Type = {contentType}");
                Console.WriteLine();
                Console.WriteLine($"Message as string = {messageString}");
                Console.WriteLine();

                var messageObj = DeserializeObject(deliveryArgs.Body, contentType, messageType);

                if (messageObj.GetType() == typeof (FirstMessage))
                {
                    var myMessage = messageObj as FirstMessage;
                    Console.WriteLine($"Data from Object: FirstMessage = {myMessage.Message}");
                }
                else if (messageObj.GetType() == typeof (SecondMessage))
                {
                    var myMessage = messageObj as SecondMessage;
                    Console.WriteLine($"Data from Object: SecondMessage = {myMessage.Message}");
                }
                else
                {
                    Console.WriteLine("Data from Object: unknown data type");
                }

                Console.WriteLine();

                _model.BasicAck(deliveryArgs.DeliveryTag, false);
            }
        }

        private object DeserializeObject(byte[] body, string contentType, string messageType)
        {
            var type = Type.GetType(messageType);
            switch (contentType)
            {
                case "application/json":
                    var jsonString = Encoding.Default.GetString(body);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, type);
                case "text/xml":
                    var xmlMessageStream = new MemoryStream();
                    xmlMessageStream.Write(body, 0, body.Length);
                    xmlMessageStream.Seek(0, SeekOrigin.Begin);
                    var xmlSerializer = new XmlSerializer(type);
                    return xmlSerializer.Deserialize(xmlMessageStream);
                case "application/octet-stream":
                    var binaryMessageStream = new MemoryStream();
                    binaryMessageStream.Write(body, 0, body.Length);
                    binaryMessageStream.Seek(0, SeekOrigin.Begin);
                    var binarySerializer = new BinaryFormatter();
                    var obj = binarySerializer.Deserialize(binaryMessageStream);
                    return obj;
                default:
                    return null;
            }
        }

        private string GetMessageString(byte[] body, string contentType)
        {
            switch (contentType)
            {
                case "application/json":
                case "text/xml":
                    return Encoding.Default.GetString(body);
                case "application/octet-stream":
                    return Convert.ToBase64String(body);
                default:
                    return string.Empty;
            }
        }
    }
}
