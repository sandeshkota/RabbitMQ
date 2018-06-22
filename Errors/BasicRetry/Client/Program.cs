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

            var sender = new RabbitSender();

            Console.WriteLine("Press enter key to send a message");

            while (true)
            {
                var input = Console.ReadLine();

                if (input.ToUpper() == "Q") break;
                Console.WriteLine($"Sending - {input}");
                sender.Send(input);
            }

            Console.ReadLine();
        }
    }
}
