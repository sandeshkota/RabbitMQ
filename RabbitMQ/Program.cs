using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var rabbitMq = new RabbitMQ();
            rabbitMq.SetUp();

            var type = args[0];
            type = "1";

            switch (type)
            {
                case "OneWayMessaging":
                case "1":
                    rabbitMq.CreateSetupForOneWayMessaging();
                    break;
                case "WorkerQueue":
                case "2":
                    rabbitMq.CreateSetupForWorkerQueues();
                    break;
                case "PublishSubscribe":
                case "3":
                    rabbitMq.CreateSetupForPublishSubscribe();
                    break;
                case "RPC":
                case "4":
                    rabbitMq.CreateSetupForRemoteProcedureCall();
                    break;
                case "Routing":
                case "A1":
                    rabbitMq.CreateSetupForRouting();
                    break;
                case "Topics":
                case "A2":
                    rabbitMq.CreateSetupForTopics();
                    break;
                case "Headers":
                case "A3":
                    rabbitMq.CreateSetupForHeaders();
                    break;
                case "ScatterGather":
                case "A4":
                    rabbitMq.CreateSetupForScatterGather();
                    break;
                case "Serialization":
                case "S":
                    rabbitMq.CreateSetupForSerialization();
                    break;
                case "MessageType":
                case "M":
                    rabbitMq.CreateSetupForMessageType();
                    break;
                case "LargeBufferedMessage":
                case "LB":
                    rabbitMq.CreateSetupForLargeBufferedMessage();
                    break;
                case "LargeChunkedMessage":
                case "LC":
                    rabbitMq.CreateSetupForLargeChunkedMessage();
                    break;
                case "BasicRetry":
                case "BR":
                    rabbitMq.CreateSetupForBasicRetry();
                    break;
                case "AdvancedRetry":
                case "AR":
                    rabbitMq.CreateSetupForAdvancedRetry();
                    break;
                case "DeadLetterQueue":
                case "DLQ":
                    rabbitMq.CreateSetupForDeadLetterQueue();
                    break;
                case "RoutingFailure":
                case "RF":
                    rabbitMq.CreateSetupForRoutingFailure();
                    break;
                case "ScheduledDelivery":
                case "SD":
                    rabbitMq.CreateSetupForScheduledDelivery();
                    break;
                default:
                    rabbitMq.CreateSetupForOneWayMessaging();
                    break;
            }

            Console.WriteLine($"Setup done for {type} Exchange Pattern");

        }



    }

}
