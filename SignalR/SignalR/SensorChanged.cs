using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace MCCC_SignalR
{
    public static class SensorChanged
    {


        [FunctionName(nameof(SensorChanged))]
        public static void Run([CosmosDBTrigger(
            databaseName: "MotherCluckers",
            collectionName: "Sensor",
            ConnectionStringSetting = "AzureCosmosUrl",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> sensors,
            ILogger log,
            [SignalR(HubName = "mccc")] IAsyncCollector<SignalRMessage> signalRMessages)
        {

            log.LogInformation("SensorChanged SignalR function started");
            foreach (var sensor in sensors)
            {

                log.LogInformation("Create hearbeat obj for message");

                dynamic model = new ExpandoObject();
                model.id = sensor.Id;
                model.name = sensor.GetPropertyValue<string>("name");
                model.type = sensor.GetPropertyValue<string>("type");
                model.heartbeatInterval = sensor.GetPropertyValue<int>("heartbeatInterval");

                signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "onSensorChanged",
                            Arguments = new[] { model }
                        });
            }

        }
    }
}
