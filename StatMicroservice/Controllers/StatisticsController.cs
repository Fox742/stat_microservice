using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public IActionResult Add([FromForm]string key, [FromForm]string eventJson,[FromForm]DateTime clientDT)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return BadRequest("Key field should not be empty or whitespace");
                }

                StatisticsRepository.WriteStatistics(key, eventJson, clientDT);
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("get")]
        public IActionResult Get(
                    string key, string field,
                    DateTime? start = null,
                    DateTime? finish = null,
                    int pageSize = -1,
                    int pageNumber = -1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return BadRequest("Key field should not be empty or whitespace");
                }

                List<Dictionary<string, string>> rawData = StatisticsRepository.ReadStatistics(key, start, finish).ToList();

                if (!string.IsNullOrWhiteSpace(field))
                    _sorter.Sort(ref rawData, field);

                IEnumerable<Dictionary<string, string>> result = rawData;

                if (pageSize > 0 && pageNumber >= 0)
                    result = result.Skip(pageSize * pageNumber).Take(pageSize).ToArray();
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("getcount")]
        public IActionResult GetCount(
            string key,
            DateTime? start = null,
            DateTime? finish = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return BadRequest("Key field should not be empty or whitespace");
                }
                return Ok(StatisticsRepository.ReadStatistics(key, start, finish).Count());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("clear")]
        public IActionResult RemoveAll()
        {
            try
            {
                StatisticsRepository.RemoveAll();
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
