using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly IConfiguration _configuration;
        public StatisticsController(ILogger<StatisticsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromForm]string key, [FromForm]string eventJson,[FromForm]DateTime? clientDT = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest("key field should not be empty or whitespace");
            }

            StatisticsRepository.WriteStatistics(key,eventJson,clientDT);
            return Ok();
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<int> Get()
        {
            return new List<int>().ToArray();
        }

    }
}
