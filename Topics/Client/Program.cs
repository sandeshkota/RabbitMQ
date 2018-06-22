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
        static Dictionary<string, string> routeDictionary = new Dictionary<string, string>();

        public static void SetUp()
        {
            routeDictionary.Add("1", "chair.high.car");
            routeDictionary.Add("2", "chair.pen.cupboard");
            routeDictionary.Add("3", "car.medium.laptop");
            routeDictionary.Add("4", "corporate.car");
            routeDictionary.Add("5", "corporate.first.second.third.unlimited");
            routeDictionary.Add("6", "corporate.medium.laptop");
            routeDictionary.Add("7", "corporate.high.cupboard");
        }

        static void Main(string[] args)
        {
            SetUp();
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();

            var messageCount = 1;
            var sender = new RabbitSender();

            Console.WriteLine("Press enter key to send a message");
            Console.WriteLine("Choose Number from below for the appropriate Routing Key:");
            foreach (var routingKey in routeDictionary)
            {
                Console.WriteLine($"{routingKey.Key} = {routingKey.Value}");
            }

            while (true)
            {
                var number = Console.ReadLine();

                if (number.ToUpper() == "Q") break;

                Console.WriteLine($"Key : {number}");
                if (string.IsNullOrEmpty(number) || !routeDictionary.ContainsKey(number)) continue;

                var routingKey = routeDictionary[number];

                var message = $"Message: {messageCount}";
                Console.WriteLine($"Sending - {message}. Routing Key - {routingKey}");
                sender.Send(message, routingKey);
                messageCount++;
            }

            Console.ReadLine();
        }
    }
}
