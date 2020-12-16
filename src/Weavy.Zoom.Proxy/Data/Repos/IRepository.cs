using System.Collections.Generic;
using System.Threading.Tasks;

namespace Weavy.Zoom.Proxy.Data.Repos
{
    /// <summary>
    /// Interface for all repos
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> AddAsync(T entity);        
    }
}
