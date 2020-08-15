using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OtherWorldBot.Entities.Json
{
    public class OthergramConfig
    {
        [JsonProperty("PostsEmoji")]
        public string PostsEmoji { get; private set; } = ":heart:";

        [JsonProperty("PostsColor")]
        public string PostsColor { get; private set; } = "FF0000";

        [JsonProperty("PostsChannelId")]
        public ulong PostsChannelId { get; private set; } = 732733851192787056;
    }
}
