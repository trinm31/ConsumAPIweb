using System.Net.Http;
using WebApplication3.Models;
using WebApplication3.Repository.IRepository;

namespace WebApplication3.Repository
{
    public class TrailRepository : Repository<Trail>, ITrailRepository
    {
        private readonly IHttpClientFactory _clientFactory;

        public TrailRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            this._clientFactory = clientFactory;
        }
    }
}