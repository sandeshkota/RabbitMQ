using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ queue processor :: Server No 1");
            Console.WriteLine("Headers: material: wood, customertype: b2b");
            Console.WriteLine();


            var queueProcessor = new RabbitConsumer() { Enabled = true };
            queueProcessor.Start();

            Console.ReadLine();
        }
    }

}
