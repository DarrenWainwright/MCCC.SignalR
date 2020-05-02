using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace MCCC_SignalR
{
    public static class OnSensorChanged
    {


        [FunctionName("OnSensorChanged")]
        public static void Run([CosmosDBTrigger(
            databaseName: "MotherCluckers",
            collectionName: "Sensor",
            ConnectionStringSetting = "AzureCosmosUrl",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> sensors, ILogger log,
            [SignalR(HubName = "mccc")] IAsyncCollector<SignalRMessage> signalRMessages)
        {

            foreach (var sensor in sensors)
            {
                var heartbeat = new Model.Heartbeat(sensor.Id,
                                         sensor.GetPropertyValue<string>("name"),
                                         sensor.GetPropertyValue<string>("type"),
                                         sensor.GetPropertyValue<int>("heartbeatInterval"));
                signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "onSensorChanged",
                            Arguments = new[] { heartbeat }
                        });
            }

        }
    }
}
