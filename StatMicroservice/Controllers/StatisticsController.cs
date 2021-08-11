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

        public async Task<IActionResult> Save(string key, string eventJson, DateTime? clientDT = null)
        {



            return Ok();
        }

    }
}
