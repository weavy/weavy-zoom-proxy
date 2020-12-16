using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Weavy.Zoom.Proxy.Models
{
    public class MeetingIn
    {
        [JsonPropertyName("meeting_id")]
        public string MeetingId { get; set; }
        public string UUID { get; set; }
    }
}
