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

            Console.WriteLine("Enter a number 1-6 and press Enter");
            int messageNumber = 1;

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;

                ProcessKeyStroke(ref messageNumber, ref key);

                if (key.Key == ConsoleKey.Enter)
                {
                    if (messageNumber == 0)
                    {
                        Console.WriteLine("Enter a number 1-6 and press Enter");
                    }
                    else
                    {
                        var message = string.Format($"Message ID: {messageNumber}");
                        Console.WriteLine($"Sending - {message}");

                        var responses = sender.Send(message, messageNumber.ToString(), 
                            new TimeSpan(0,0,0,30), 0);

                        Console.WriteLine();
                        Console.WriteLine($"{responses.Count} replies received");
                        Console.WriteLine();
                        Console.WriteLine("Listing Responses:");
                        foreach (var response in responses)
                            Console.WriteLine($"Response - {response}");

                        Console.WriteLine();
                        Console.WriteLine("Enter a number 1-6 and press Enter");
                        messageCount++;
                    }
                }

                
            }

            
        }

        private static void ProcessKeyStroke(ref int messageNumber, ref ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    messageNumber = 1;
                    break;
                case ConsoleKey.D2:
                    messageNumber = 2;
                    break;
                case ConsoleKey.D3:
                    messageNumber = 3;
                    break;
                case ConsoleKey.D4:
                    messageNumber = 4;
                    break;
                case ConsoleKey.D5:
                    messageNumber = 5;
                    break;
                case ConsoleKey.D6:
                    messageNumber = 6;
                    break;
                case ConsoleKey.Enter:
                    break;
                default:
                    messageNumber = 0;
                    break;
            }
        }
    }
}
