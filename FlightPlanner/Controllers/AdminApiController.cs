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
        [HttpGet, Authorize]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            var flight = FlightStorage.GetFlight(id);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }

        [HttpDelete, Authorize]
        [Route("flights/{id}")]
        public IActionResult DeleteFlights(int id)
        {
            FlightStorage.DeleteFlights(id);

            return Ok();
        }

        [HttpPut, Authorize]
        [Route("flights")]
        public IActionResult PutFlights(AddFlightRequest request)
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
