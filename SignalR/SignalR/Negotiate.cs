using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace SignalR
{
    public static class Negotiate
    {

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Run(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "mccc")] SignalRConnectionInfo connectionInfo,
            ILogger log)
        {
            return connectionInfo;
            //return new OkObjectResult(connectionInfo);
        }

    }
}
