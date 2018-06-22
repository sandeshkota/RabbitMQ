using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FailureServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ queue processor");
            Console.WriteLine();

            var queueProcessor = new FailureConsumer() { Enabled = true };
            queueProcessor.Start();

            Console.ReadLine();
        }
    }
}
