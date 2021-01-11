using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weavy.Zoom.Proxy.Attributes;
using Weavy.Zoom.Proxy.Data.Repos;
using Weavy.Zoom.Proxy.Models;
using Weavy.Zoom.Proxy.Services;

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
        private readonly IRestService _restService;

        public EventsController(IZoomEventRepository zoomEventRepository, IConfiguration configuration, IRestService restService)
        {
            _zoomEventRepository = zoomEventRepository;
            _configuration = configuration;
            _restService = restService;
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
            switch (zoomEvent.Event)
            {
                // user uninstalled zoom app
                case "app_deauthorized":                 
                    if (zoomEvent.Payload.UserDataRetention == "false")
                    {
                        // call Zoom Data Compliance
                        var json = SerializeComplianceData(zoomEvent);
                        await _restService.Post("https://api.zoom.us/oauth/data/compliance", json);
                    }
                    break;
                default:
                    // create a new event and add it to the database
                    var meeting = new ZoomEvent
                    {
                        MeetingId = zoomEvent.Payload.Object.Id.ToString(),
                        UUID = zoomEvent.Payload.Object.Uuid,
                        EventType = zoomEvent.Event,
                        MetaData = ResolveMetaData(zoomEvent)
                    };

                    await _zoomEventRepository.AddAsync(meeting);

                    break;
            }
          
            // 200 OK must be returned within 3 seconds, otherwise Zoom will treat the request as unsuccessful and try again in 5 minutes.
            return Ok();            
        }

        /// <summary>
        /// Returns status for all requested meetings
        /// </summary>
        /// <param name="meetings"></param>
        /// <returns></returns>
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

        #region private

        /// <summary>
        /// Helper for constructing the compliance json data sent to Zoom after user deauthorization
        /// </summary>
        /// <param name="zoomEvent"></param>
        /// <returns></returns>
        private string SerializeComplianceData(ZoomNotification zoomEvent)
        {
            var json = new
            {
                client_id = zoomEvent.Payload.ClientId,
                user_id = zoomEvent.Payload.UserId,
                account_id = zoomEvent.Payload.AccountId,
                deauthorization_event_received = new
                {
                    user_data_retention = zoomEvent.Payload.UserDataRetention,
                    account_id = zoomEvent.Payload.AccountId,
                    user_id = zoomEvent.Payload.UserId,
                    signature = zoomEvent.Payload.Signature,
                    deauthorization_time = zoomEvent.Payload.DeauthorizationTime,
                    client_id = zoomEvent.Payload.ClientId
                },
                compliance_completed = true
            };

            return JsonConvert.SerializeObject(json);
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

        #endregion
    }
}
