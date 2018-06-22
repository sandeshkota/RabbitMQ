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
        static Dictionary<string, string> materialDictionary = new Dictionary<string, string>();
        static Dictionary<string, string> customerTypeDictionary = new Dictionary<string, string>();
        private static int messageCount = 1;

        static void Setup()
        {
            materialDictionary.Add("1", "wood");
            materialDictionary.Add("2", "metal");

            customerTypeDictionary.Add("1", "b2c");
            customerTypeDictionary.Add("2", "b2b");
        }

        static void Main(string[] args)
        {
            Setup();
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();

            var sender = new RabbitSender();

            Console.WriteLine("Press enter key to send a message");
            Console.WriteLine("Format to enter: {#material} - {#customertype}");

            Console.WriteLine("Materials:");
            foreach (var material in materialDictionary) {
                Console.WriteLine($"{material.Key} = {material.Value}");
            }

            Console.WriteLine();
            Console.WriteLine("Customer Type:");
            foreach (var customerType in customerTypeDictionary)
            {
                Console.WriteLine($"{customerType.Key} = {customerType.Value}");
            }

            while (true)
            {
                var input = Console.ReadLine();

                if (input.ToUpper() == "Q") break;
                if(!input.Contains('-')) continue;

                var inputData = input.Split('-');
                var materialCode = inputData[0].Trim();
                var customerTypeCode = inputData[1].Trim();

                if (!materialDictionary.ContainsKey(materialCode) ||
                   !customerTypeDictionary.ContainsKey(customerTypeCode))
                {
                    Console.WriteLine("Invalid Input");
                    continue;
                }

                var material = materialDictionary[materialCode];
                var customerType = customerTypeDictionary[customerTypeCode];
                Console.WriteLine($"Materail : {material}, Customer Type: {customerType}");

                var headers = new Dictionary<string, string>
                {
                    {"material", material},
                    {"customerType", customerType}
                };

                var message = $"Message: {messageCount}";
                sender.Send(message, headers);
                messageCount++;
            }

            Console.ReadLine();
        }
    }
}
