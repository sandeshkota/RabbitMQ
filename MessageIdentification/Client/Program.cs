using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Contract;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();

            var messageCount = 1;
            var sender = new RabbitSender();

            Console.WriteLine("Press enter key to send a message");
            Console.WriteLine("Select the number for the Format - number of Message Type, and press Enter");
            Console.WriteLine("1 = JSON");
            Console.WriteLine("2 = XML");
            Console.WriteLine("3 = Binary");
            Console.WriteLine();
            Console.WriteLine("1 = FirstMessage");
            Console.WriteLine("2 = SecondMessage");
            Console.WriteLine();

            while (true)
            {
                var input = Console.ReadLine();

                if (input.ToUpper() == "Q") break;
                if(!input.Contains("-")) continue;

                var data = input.Split('-');
                var format = data[0];
                var messageType = data[1];

                if (format != "1" && format != "2" && format != "3") continue;
                if (messageType != "1" && messageType != "2") continue;


                object messageObj = null;
                if (messageType == "1")
                    messageObj = new FirstMessage {Message = $"Message: {messageCount}"};
                else
                    messageObj = new SecondMessage { Message = $"Message: {messageCount}" };

                var messageBuffer = SerializeMessage(messageObj, format);

                Console.WriteLine($"Foramt - {format}, Sending - {messageObj}");
                sender.Send(messageBuffer, GetContentType(format), GetMessageType(messageObj));
                messageCount++;
            }

            Console.ReadLine();
        }

        private static string GetMessageType(object message)
        {
            return message.GetType().AssemblyQualifiedName;
        }

        private static string GetContentType(string format)
        {
            switch (format)
            {
                case "1":
                case "JSON":
                    return "application/json";
                case "2":
                case "XML":
                    return "text/xml";
                case "3":
                case "Binary":
                    return "application/octet-stream";
                default:
                    return String.Empty;
            }
        }

        private static byte[] SerializeMessage(object messageObj, string format)
        {
            switch (format)
            {
                case "1":
                case "JSON":
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(messageObj);
                    return Encoding.Default.GetBytes(jsonString);
                case "2":
                case "XML":
                    var xmlMessageStream = new MemoryStream();
                    var xmlSerializer = new XmlSerializer(messageObj.GetType());
                    xmlSerializer.Serialize(xmlMessageStream, messageObj);
                    xmlMessageStream.Flush();
                    xmlMessageStream.Seek(0, SeekOrigin.Begin);
                    return xmlMessageStream.GetBuffer();
                case "3":
                case "Binary":
                    var binaryMessageStream = new MemoryStream();
                    var binarySerializer = new BinaryFormatter();
                    binarySerializer.Serialize(binaryMessageStream, messageObj);
                    binaryMessageStream.Flush();
                    binaryMessageStream.Seek(0, SeekOrigin.Begin);
                    return binaryMessageStream.GetBuffer();
                default:
                    return null;
            }
        }

    }
}
