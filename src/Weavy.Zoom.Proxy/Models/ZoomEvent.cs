using System;

namespace Weavy.Zoom.Proxy.Models
{
    public class ZoomEvent
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MeetingId { get; set; }
        public string UUID { get; set; }
        public string EventType { get; set; }
        public string MetaData { get;  set; }
    }
}
