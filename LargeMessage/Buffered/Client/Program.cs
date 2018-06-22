using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private const string InputFile = "BigFile.txt";

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
                    // Read File
                    Console.WriteLine($"Reading file - {InputFile}");
                    byte[] messageBuffer = File.ReadAllBytes(InputFile);

                    // Send Message
                    Console.WriteLine($"Sending large message - {messageBuffer.Length}");
                    sender.Send(messageBuffer);
                    messageCount++;
                    Console.WriteLine("Message sent");
                }
            }

            Console.ReadLine();
        }
    }
}
