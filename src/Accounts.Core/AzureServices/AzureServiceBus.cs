using Abp;
using Abp.Dependency;
using Abp.Domain.Services;
using Accounts.Core.Invoicing;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Accounts.AzureServices
{
    public class AzureServiceBus : DomainService, IAzureServiceBus
    {
        private readonly IInvoicingService InvoicingService;
        private readonly OAuth2Client OAuth2Client;
        private readonly IConfiguration Configuration;

        public AzureServiceBus(IInvoicingService invoicingService, OAuth2Client oAuth2Client, IConfiguration configuration)
        {
            InvoicingService = invoicingService;
            OAuth2Client = oAuth2Client;
            Configuration = configuration;
        }

        public void ServiceBusListener()
        {
            try
            {
                var queueAceessKey = Configuration.GetSection("ServiceBus:QueueAceessKey").Value;
                ServiceBusConnectionStringBuilder conStr = new ServiceBusConnectionStringBuilder(queueAceessKey);
                conStr.EntityPath = Configuration.GetSection("ServiceBus:Queue").Value;
                QueueClient client = new QueueClient(conStr, ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);
                var messageHandler = new MessageHandlerOptions(ListenerExceptionHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                };
                client.RegisterMessageHandler(ReceiveMessageFromQ, messageHandler);
            }
            catch (Exception exe)
            {
                Console.WriteLine("{0}", exe.Message);
            }
        }
        public async Task ReceiveMessageFromQ(Message message, CancellationToken token)
        {
            string requestContent = Encoding.UTF8.GetString(message.Body);
            var eventGrid = JsonConvert.DeserializeObject<EventGridEvent>(requestContent.ToString());
            if (string.Equals(eventGrid.EventType, "Microsoft.EventGrid.SubscriptionValidationEvent", StringComparison.OrdinalIgnoreCase))
            {
                var data = eventGrid.Data as JObject;
                var eventData = data.ToObject<SubscriptionValidationEventData>();
                var responseData = new SubscriptionValidationResponse
                {
                    ValidationResponse = eventData.ValidationCode
                };

                if (responseData.ValidationResponse != null)
                {
                    await Task.CompletedTask;
                }
            }
            else
            {
                var data = eventGrid.Data.ToString();
                var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
                if (isConnectionEstablished)
                {
                    await InvoicingService.SyncInvoice(data);
                }
            }
            await System.Threading.Tasks.Task.CompletedTask;
        }
        public Task ListenerExceptionHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine("{0}", exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}
