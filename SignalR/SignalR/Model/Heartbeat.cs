using Newtonsoft.Json;

namespace MCCC_SignalR.Model
{
    public class Heartbeat
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("heartbeatInterval")]
        public int HeartbeatInterval { get; set; }

        public Heartbeat(string id, string name, string type, int heartbeatInterval)
        {
            Id = id;
            Name = name;
            Type = type;
            HeartbeatInterval = heartbeatInterval;
        }
    }
}
