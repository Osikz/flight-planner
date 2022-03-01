using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private static readonly object _customerLocker = new object();

        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {
            lock (_customerLocker)
            {
                var airports = FlightStorage.SearchAirports(search);
                return Ok(airports);
            }
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchFlightsRequest req)
        {
            lock (_customerLocker)
            {
                if (!FlightStorage.IsValidSearch(req))
                {
                    return BadRequest();
                }

                return Ok(FlightStorage.SearchFlight(req));
            }
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult FindFlightById(int id)
        {
            lock (_customerLocker)
            {
                var flight = FlightStorage.GetFlight(id);

                if (flight == null)
                {
                    return NotFound();
                }

                return Ok(flight);
            }
        }
    }
}
