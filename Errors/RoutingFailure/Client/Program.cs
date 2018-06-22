using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Client
{
    class Program
    {
        static readonly Dictionary<string, string> routingDictionary = new Dictionary<string, string>()
        {
            { "1","Apples"},
            { "2","Bananas"},
            { "3","Oranges"}
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();

            var sender = new RabbitSender();

            Console.WriteLine("Select a number & Press enter key to send a message");
            foreach (var routing in routingDictionary) {
                Console.WriteLine($"{routing.Key} = {routing.Value}");
            }


            while (true)
            {
                var input = Console.ReadLine();

                if (input.ToUpper() == "Q") break;
                if (input.ToUpper() != "1" && input.ToUpper() == "2" && input.ToUpper() == "3") continue;

                var routingKey = routingDictionary[input];
                Console.WriteLine($"Sending - {routingKey}");
                sender.Send(routingKey, routingKey);
            }

            Console.ReadLine();
        }
    }
}
