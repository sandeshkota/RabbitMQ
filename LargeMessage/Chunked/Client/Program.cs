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
        private const int ChunkSize = 4096;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();

            var sender = new RabbitSender();
            Console.WriteLine("Press enter key to send a message");

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;

                if (key.Key == ConsoleKey.Enter)
                {

                    var outputFileName = $"{Guid.NewGuid()}";
                    var fileStream = File.OpenRead(InputFile);
                    var streamReader = new StreamReader(fileStream);
                    int remaining = (int) fileStream.Length;
                    int length = (int) fileStream.Length;
                    var messageCount = 0;
                    var endOfSequence = false;
                    byte[] buffer;

                    while (true)
                    {
                        if (remaining <= 0) break;

                        int read = 0;
                        if (remaining > ChunkSize)
                        {
                            buffer = new byte[ChunkSize];
                            read = fileStream.Read(buffer, 0, ChunkSize);
                        }
                        else
                        {
                            buffer = new byte[remaining];
                            read = fileStream.Read(buffer, 0, remaining);
                            endOfSequence = true;
                        }

                        // Send Message
                        Console.WriteLine($"Sending chunk message - Index = {messageCount}; " +
                                          $"Length =  {length}");
                        sender.Send(buffer, outputFileName, messageCount, endOfSequence);

                        messageCount++;
                        remaining = remaining - read;
                    }
                    Console.WriteLine($"Completed sending {messageCount} chunks");
                    
                }
            }

            Console.ReadLine();
        }
    }
}
