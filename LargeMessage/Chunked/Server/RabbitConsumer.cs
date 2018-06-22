using System;
using System.Collections.Generic;
using System.IO;
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
        private IModel _model;
        private const string _exchangeName = "CashAccount.MyExchange";
        private const string _queueName = "CashAccount.MyQueue";
        private readonly int _port = 0;

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

            while (true)
            {
                var deliveryArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                Console.WriteLine($"Received message - Size: {deliveryArgs.Body.Length}");

                var pathProperty = (byte[])deliveryArgs.BasicProperties.Headers["OutputFileName"];
                var outputPath = Encoding.Default.GetString(pathProperty);
                var sequenceNumber = (int)deliveryArgs.BasicProperties.Headers["SequenceNumber"];
                var endOfSequence = (bool)deliveryArgs.BasicProperties.Headers["EndOfSequence"];

                outputPath += ".inprogress";

                using (var fileStream = new FileStream(outputPath, FileMode.Append, FileAccess.Write))
                {
                    fileStream.Write(deliveryArgs.Body, 0, deliveryArgs.Body.Length);
                    fileStream.Flush();
                }
                Console.WriteLine($"Message saved to disk - Sequence No = {sequenceNumber}");

                if (endOfSequence)
                {
                    Console.WriteLine("Received last message, so renaming the file");
                    var newOutputPath = Path.ChangeExtension(outputPath, ".completed");
                    File.Move(outputPath, newOutputPath);
                }

                _model.BasicAck(deliveryArgs.DeliveryTag, false);
            }
        }
    }
}
