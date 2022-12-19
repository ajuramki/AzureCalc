using AngouriMath;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using AzureCalc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureCalc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.QueueReadMessage = null;
            return View();
        }

        public IActionResult Privacy()
        {
            IEnumerable<Calculator> calc = null;
            using (var context = new AjWebAppDatabaseContext())
            {
                calc = context.Calculators.ToList();
            }
            return View(calc);
        }


        public IActionResult LoadPartial(string fvalue, string svalue, string operation)
        {
            Calculator calc = null;
            using (var context = new AjWebAppDatabaseContext())
            {
                calc = context.Calculators.Where(s => s.FirstValue == fvalue && s.SecondValue == svalue &&
                s.Operator == operation).FirstOrDefault();
            }
            ViewBag.TotalValue = calc.Total;
            ViewBag.FirstValue = calc.FirstValue;
            ViewBag.SecondValue = calc.SecondValue;
            ViewBag.operation = calc.Operator;
            return PartialView("CalcDetails");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult sendMessage(string fvalue, string svalue, string operation)
        {
            var message = fvalue + ":" + operation + ":" + svalue;
            ServiceBusClient client;
            // the sender used to publish messages to the queue
            ServiceBusSender sender;
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            //Endpoint=sb://aj-service-bus-1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oNJz9M2G6E1LKgBDQI4o2kEM2TpgTfyCNJ4n2heNlok=
            client = new ServiceBusClient(
                "aj-service-bus-1.servicebus.windows.net",
                new DefaultAzureCredential(),
                clientOptions);

            //client = new ServiceBusClient("Endpoint = sb://aj-service-bus-1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oNJz9M2G6E1LKgBDQI4o2kEM2TpgTfyCNJ4n2heNlok=",
            //    clientOptions);
            sender = client.CreateSender("myservicequeue");
            ServiceBusMessage messageBatch = new(message);
            try
            {
                //Use the producer client to send the batch of messages to the Service Bus queue
                //await 
                sender.SendMessageAsync(messageBatch).ConfigureAwait(false);
                Entity expr = fvalue + operation + svalue;
                var totalvalue = expr.EvalNumerical().ToString();
                using (var context = new AjWebAppDatabaseContext())
                {
                    var calcout = new Calculator()
                    {
                        FirstValue = fvalue,
                        SecondValue = svalue,
                        Operator = operation,
                        Total = totalvalue
                    };
                    context.Calculators.Add(calcout);
                    context.SaveChanges();
                }
            }
            finally
            {
                //await 
                sender.DisposeAsync();
                client.DisposeAsync();
            }
            return new EmptyResult();
        }

        public IActionResult readMessages()
        {
            //    public async Task<IActionResult> readMessages()
            //{
            ServiceBusClient client;
            // the processor that reads and processes messages from the queue
            ServiceBusProcessor processor;
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            client = new ServiceBusClient("aj-service-bus-1.servicebus.windows.net",
                new DefaultAzureCredential(), clientOptions);
            processor = client.CreateProcessor("myservicequeue", new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            });
            try
            {
                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;
                processor.StartProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
            ViewBag.QueueReadMessage = "Service queue message deleted";
            return new EmptyResult();

        }


        // handle received messages
        public Task MessageHandler(ProcessMessageEventArgs args)
        {
            //    public async Task<ActionResult> MessageHandler(ProcessMessageEventArgs args)
            //{
            string body = args.Message.Body.ToString();
            // complete the message. message is deleted from the queue. 
            args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
            ViewBag.QueueReadMessage = "Service queue message deleted";
            return Task.CompletedTask;
        }


        // handle any errors when receiving messages
        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            ViewBag.Error = args.Exception.ToString();
            return Task.CompletedTask;
        }


    }
}