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
        private Sorter _sorter; 
        public StatisticsController(ILogger<StatisticsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _sorter = new Sorter();
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
        public IEnumerable<Dictionary<string, string>> Get(
            string key, string field,
            DateTime? start = null,
            DateTime? finish = null, 
            int pageSize=-1,
            int pageNumber=-1)
        {
            List<Dictionary<string, string>> rawData = StatisticsRepository.ReadStatistics(key, start, finish).ToList();
            _sorter.Sort(ref rawData, field);


            return rawData.ToArray();
        }

        [HttpPost]
        [Route("clear")]
        public IActionResult RemoveAll()
        {
            StatisticsRepository.RemoveAll();
            return Ok();
        }

    }
}
