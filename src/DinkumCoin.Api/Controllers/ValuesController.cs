using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DinkumCoin.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        //private readonly ILogger<ValuesController> _logger;

        //public ValuesController(ILogger<ValuesController> logger)
        //{
        //    _logger = logger;
        //}
        //// GET api/values
        [HttpGet]
        public async Task<dynamic> Get()
        {
            //_logger.LogInformation($"Get values executed at {DateTime.Now}");

            return new
            {
                ExtendedCheck = new
                {
                    Status = "Success"
                }
            };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}