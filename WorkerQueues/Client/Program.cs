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

                if (key.Key == ConsoleKey.Enter)
                {
                    var message = $"Message: {messageCount}";
                    Console.WriteLine("Sending - {0}", message);
                    sender.Send(message);
                    messageCount++;
                }
            }

            Console.ReadLine();
        }
    }
}