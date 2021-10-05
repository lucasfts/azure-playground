using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Collections.Generic;

namespace RedisQuickstart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IDatabase cache;

        public CacheController(IDatabase cache) 
            => this.cache = cache;

        [HttpGet("{key}")]
        public string Get(string key) 
            => cache.StringGet(key);

        [HttpPost]
        public void Post([FromBody] KeyValuePair<string, string> keyValue) 
            => cache.StringSet(keyValue.Key, keyValue.Value);
    }
}
