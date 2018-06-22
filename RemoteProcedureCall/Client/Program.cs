using System;
using System.Collections.Generic;
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

                if (key.Key != ConsoleKey.Enter) continue;
                var message = $"Message: {messageCount}";
                Console.WriteLine("Sending - {0}", message);

                var response = sender.Send(message, new TimeSpan(0,0,3,0));

                Console.WriteLine("Response - {0}", response);
                messageCount++;
            }

            Console.ReadLine();
        }
    }
}
