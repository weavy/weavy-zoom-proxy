using System.Text.Json.Serialization;

namespace Weavy.Zoom.Proxy.Models
{
    /// <summary>
    /// Model representing a Zoom webhook event
    /// </summary>
    public class ZoomNotification
    {

        /// <summary>
        /// The type of event
        /// </summary>
        [JsonPropertyName("event")]
        public string Event { get; set; }

        /// <summary>
        /// The payload data
        /// </summary>
        [JsonPropertyName("payload")]
        public ZoomEventPayload Payload { get; set; }
    }


    /// <summary>
    /// Model representing a Zoom webhook event payload
    /// </summary>
    public class ZoomEventPayload
    {
        /// <summary>
        /// The account associated with the event
        /// </summary>
        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// The payload object
        /// </summary>
        [JsonPropertyName("object")]
        public ZoomEventPayloadObject Object { get; set; }
    }


    /// <summary>
    /// A model representing the payload object
    /// </summary>
    public class ZoomEventPayloadObject
    {

        /// <summary>
        /// The id of the payload
        /// </summary>
        [JsonPropertyName("id")]
        public object Id { get; set; }

        /// <summary>
        /// The unique id
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// The host id
        /// </summary>
        [JsonPropertyName("host_id")]
        public string HostId { get; set; }

        /// <summary>
        /// The url of a recording (in case of recording.completed event)
        /// </summary>
        [JsonPropertyName("share_url")]
        public string RecordingUrl { get; set; }
    }
}
