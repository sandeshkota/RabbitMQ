using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;

                if (key.Key == ConsoleKey.Enter)
                {
                    var routingKey = new Random().Next(0, 4).ToString(CultureInfo.InvariantCulture);

                    var message = $"Message: {messageCount}";
                    Console.WriteLine($"Sending - {message}. Routing Key - {routingKey}");
                    sender.Send(message, routingKey);
                    messageCount++;
                }
            }

            Console.ReadLine();
        }
    }
}
