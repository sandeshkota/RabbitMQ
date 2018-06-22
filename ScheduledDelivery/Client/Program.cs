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

            Console.WriteLine("Enter the time delay & Press enter key to send a message");

            while (true)
            {
                var input = Console.ReadLine();
                int delay;

                if (input.ToUpper() == "Q") break;
                if (!int.TryParse(input, out delay)) continue;

                var message = $"Message: {messageCount}";
                Console.WriteLine("Sending - {0}", message);
                sender.Send(message, input);
                messageCount++;
            }
        }
    }
}
