// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.Dynamic;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MCCC_SignalR
{
    public static class SignalR
    {

        const string HUB_NAME = "mccc";

        [FunctionName("Heartbeat")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log,
                    [SignalR(HubName = HUB_NAME)] IAsyncCollector<SignalRMessage> signalRMessages)
        {

            try
            {
                dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(eventGridEvent.Data.ToString());
                Model.Heartbeat hb = new Model.Heartbeat(data.name, data.type, (int)data.heartbeat_interval);


                signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "heartbeatReceived",
                        Arguments = new[] { hb }
                    });
            }
            catch (System.Exception ex)
            {

                throw new InvalidOperationException("Could not return signalR", ex);
            }


        }
    }
}
