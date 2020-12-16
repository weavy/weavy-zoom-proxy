using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weavy.Zoom.Proxy.Attributes;
using Weavy.Zoom.Proxy.Data.Repos;
using Weavy.Zoom.Proxy.Models;

namespace Weavy.Zoom.Proxy.Controllers.Api
{
    /// <summary>
    /// Api controller for Zoom notification events and Weavy polling endpoint
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IZoomEventRepository _zoomEventRepository;
        private readonly IConfiguration _configuration;

        public EventsController(IZoomEventRepository zoomEventRepository, IConfiguration configuration)
        {
            _zoomEventRepository = zoomEventRepository;
            _configuration = configuration;
        }
                
        
        /// <summary>
        /// Zoom webhook notification endpoint. 
        /// This is the endpoint that is specified in the Zoom app's Event notification url.
        /// </summary>
        /// <param name="zoomEvent">The event data from Zoom</param>
        /// <returns>200 OK</returns>
        [HttpPost]
        [TokenAuthorization]
        public async Task<IActionResult> Zoom(ZoomNotification zoomEvent)
        {                            
            // create a new event and add it to the database
            var meeting = new ZoomEvent
            {
                MeetingId = zoomEvent.Payload.Object.Id.ToString(),
                UUID = zoomEvent.Payload.Object.Uuid,
                EventType = zoomEvent.Event,
                MetaData = ResolveMetaData(zoomEvent)
            };                

            await _zoomEventRepository.AddAsync(meeting);

            // 200 OK must be returned within 3 seconds, otherwise Zoom will treat the request as unsuccessful and try again in 5 minutes.
            return Ok();            
        }

        [HttpPost]
        [TokenAuthorization]
        [Route("poll")]
        public async Task<IActionResult> Poll(IEnumerable<MeetingIn> meetings)
        {         
            var result = new List<ZoomEvent>();

            foreach (var meeting in meetings)
            {
                var zoomEvents = await _zoomEventRepository.GetByIdAsync(meeting.MeetingId, meeting.UUID);
                if (zoomEvents.Any())
                {
                    result.AddRange(zoomEvents);
                }
            }

            return Ok(result);         
        }

        /// <summary>
        /// Helper for getting the metadata from an event where it is applicable
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private string ResolveMetaData(ZoomNotification notification)
        {
            switch (notification.Event)  
            {
                case "recording.completed":
                    return notification.Payload.Object.RecordingUrl;
                default:
                    return null;
            }
        }
    }
}
