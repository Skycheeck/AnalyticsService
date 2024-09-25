using System;
using Newtonsoft.Json;

namespace DTO
{
    [Serializable]
    public struct AnalyticsEventDTO
    {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("data")]
        public string Data;

        public AnalyticsEventDTO(string type, string data)
        {
            Type = type;
            Data = data;
        }
    }
}