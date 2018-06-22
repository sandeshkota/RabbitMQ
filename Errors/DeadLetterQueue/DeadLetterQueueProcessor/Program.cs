using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadLetterQueueProcessor;

namespace Server
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ queue processor");
            Console.WriteLine();

            var queueProcessor = new DeadLetterConsumer() { Enabled = true };
            queueProcessor.Start();

            Console.ReadLine();
        }
    }

}
