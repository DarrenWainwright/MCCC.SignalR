using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace SignalR
{
    public static class SensorDataChanged
    {
        [FunctionName(nameof(SensorDataChanged))]
        public static void Run([CosmosDBTrigger(databaseName: "MotherCluckers",
            collectionName: "SensorData",
            ConnectionStringSetting = "AzureCosmosUrl",
            LeaseCollectionName = "leases",  CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input,
            ILogger log,
            [SignalR(HubName = "mccc")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            log.LogInformation("TemperatureChanged SignalR function started");

            input.ToList().ForEach(inp =>
            {
                try
                {
                    log.LogInformation("Attempt to create obj");


                    var c = inp.GetPropertyValue<double>("celcius");
                    var f = inp.GetPropertyValue<double>("fahrenheit");
                    var h = inp.GetPropertyValue<double>("humidity");
                    dynamic model = new ExpandoObject();
                    model.name = inp.GetPropertyValue<string>("name");
                    if (c != 0)
                        model.celcius = c;
                    if (f != 0)
                        model.fahrenheit = f;
                    if (h != 0)
                        model.humidity = h;


                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        Target = "onSensorDataChanged",
                        Arguments = new[] { model }
                    });
                }
                catch (Exception ex)
                {
                    //TODO .. finish this..
                    log.LogError(ex, ex.Message);
                }
            });


        }
    }
}
