using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Weavy.Zoom.Proxy.Models;

namespace Weavy.Zoom.Proxy.Data.Repos
{
 
    /// <summary>
    /// ZoomEvent repo inteface
    /// </summary>
    public interface IZoomEventRepository : IRepository<ZoomEvent>
    {
        Task<IEnumerable<ZoomEvent>> GetByIdAsync(string meetingId, string UUID);        
    }

    /// <summary>
    /// Zoom repo implementation
    /// </summary>
    public class ZoomEventRepository : IZoomEventRepository
    {
        private readonly IConfiguration _configuration;

        public ZoomEventRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Add a new Zoom event
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(ZoomEvent entity)
        {
            entity.CreatedAt = DateTime.Now;
            var sql = "Insert into ZoomEvents (CreatedAt, MeetingId, UUID, EventType, MetaData) VALUES (@CreatedAt,@MeetingId,@UUID,@EventType,@MetaData)";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        
        /// <summary>
        /// Get all available events
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ZoomEvent>> GetAllAsync()
        {
            var sql = "SELECT * FROM ZoomEvents";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ZoomEvent>(sql);
                return result;
            }
        }

        /// <summary>
        /// Get an event by the meeting id and the unique id (UUID)
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="UUID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ZoomEvent>> GetByIdAsync(string meetingId, string UUID)
        {
            var sql = "SELECT * FROM ZoomEvents WHERE MeetingId = @meetingId AND UUID = @UUID";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ZoomEvent>(sql, new { meetingId, UUID});
                return result;
            }
        }
                
    }
}
