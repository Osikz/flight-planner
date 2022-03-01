using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class TestingApiController : ControllerBase
    {
        private static readonly object _testLocker = new object();

        [HttpPost]
        [Route("clear")]
        public IActionResult Clear()
        {
            lock (_testLocker)
            {
                FlightStorage.ClearFlights();
                return Ok();
            }
        }
    }
}
