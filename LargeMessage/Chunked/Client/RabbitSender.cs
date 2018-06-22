using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Client
{
    class RabbitSender
    {
        private readonly string _hostName = "server.in.server.com";
        private readonly string _userName = "userName";
        private readonly string _password = "passWord";
        private readonly string _virtualHost = "hostName";
        private IModel _model;
        private const string _exchangeName = "CashAccount.MyExchange";
        private readonly int _port = 0;

        public RabbitSender()
        {
            DisplaySettings();
            SetUpRabbitMq();
        }

        private void DisplaySettings()
        {
            Console.WriteLine("ExchangeName: {0}", _exchangeName);
        }

        private void SetUpRabbitMq()
        {
            var connectionFactory = new ConnectionFactory { HostName = _hostName, UserName = _userName, Password = _password };
            if (string.IsNullOrEmpty(_virtualHost) == false) connectionFactory.VirtualHost = _virtualHost;
            if (_port > 0) connectionFactory.Port = _port;

            var connection = connectionFactory.CreateConnection();
            _model = connection.CreateModel();
        }


        public void Send(byte[] messageBuffer, string outputFileName, int sequenceNumber, bool endOfSequence)
        {
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object>
            {
                {"OutputFileName", outputFileName},
                {"SequenceNumber", sequenceNumber},
                {"EndOfSequence", endOfSequence}
            };

            _model.BasicPublish(_exchangeName, "", properties, messageBuffer);
        }
    }
}
