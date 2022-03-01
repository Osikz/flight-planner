using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private static readonly object _adminLocker = new object();

        [HttpGet, Authorize]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            lock (_adminLocker)
            {
                var flight = FlightStorage.GetFlight(id);

                if (flight == null)
                {
                    return NotFound();
                }

                return Ok(flight);
            }
        }

        [HttpDelete, Authorize]
        [Route("flights/{id}")]
        public IActionResult DeleteFlights(int id)
        {
            lock (_adminLocker)
            {
                FlightStorage.DeleteFlights(id);

                return Ok();
            }
        }

        [HttpPut, Authorize]
        [Route("flights")]
        public IActionResult PutFlights(AddFlightRequest request)
        {
            lock (_adminLocker)
            {
                if (!FlightStorage.IsValid(request))
                {
                    return BadRequest();
                }

                if (FlightStorage.Exists(request))
                {
                    return Conflict();
                }

                var flight = FlightStorage.AddFlight(request);

                return Created("", flight);
            }
        }
    }
}
