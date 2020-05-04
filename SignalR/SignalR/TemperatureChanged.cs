using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace SignalR
{
    public static class TemperatureChanged
    {
        [FunctionName(nameof(TemperatureChanged))]
        public static void Run([CosmosDBTrigger(databaseName: "MotherCluckers",
            collectionName: "SensorData",
            ConnectionStringSetting = "AzureCosmosUrl",
            LeaseCollectionName = "leases",  CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input,
            ILogger log,
            [SignalR(HubName = "mccc")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            log.LogInformation("TemperatureChanged SignalR function started");
            /// Not filtering any of the SensorData docuemnts here.
            /// Event Grid will filter traffic to this function

            input.Where(p => !p.GetPropertyValue<double>("celcius").IsNull()).ToList().ForEach(inp =>
            {
                try
                {
                    log.LogInformation("Attempt to create obj");

                    dynamic model = new ExpandoObject();
                    model.sensorName = inp.GetPropertyValue<string>("name");
                    model.celcius = inp.GetPropertyValue<double>("celcius");
                    model.heartbeatInterval = inp.GetPropertyValue<double>("fahrenheit");


                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        Target = "onTemperatureChanged",
                        Arguments = new[] { model }
                    });
                }
                catch (System.Exception ex)
                {

                    throw new InvalidOperationException("Could not send message.", ex);
                }


            });


        }
    }
}