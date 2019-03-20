using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MachineCalculator.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok();
        }
    }
}