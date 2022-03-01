using System.Linq;
using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [EnableCors]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _flightLocker = new object();

        public AdminApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet, Authorize]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            lock (_flightLocker)
            {
                var flight = _context.Flights
                    .Include(f => f.From)
                    .Include(f => f.To)
                    .SingleOrDefault(f => f.Id == id);

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
            lock (_flightLocker)
            {
                var flight = _context.Flights
                    .Include(f => f.From)
                    .Include(f => f.To)
                    .SingleOrDefault(f => f.Id == id);

                if (flight != null)
                {
                    _context.Flights.Remove(flight);
                    _context.SaveChanges();
                }

                return Ok();
            }
        }

        [HttpPut, Authorize]
        [Route("flights")]
        public IActionResult PutFlights(AddFlightRequest request)
        {
            lock (_flightLocker)
            {
                if (!FlightStorage.IsValid(request))
                {
                    return BadRequest();
                }

                if (FlightExistsInStorage(request))
                {
                    return Conflict();
                }

                var flight = FlightStorage.ConvertToFlight(request);

                _context.Flights.Add(flight);
                _context.SaveChanges();

                return Created("", flight);
            }
        }

        private bool FlightExistsInStorage(AddFlightRequest request)
        {
            lock (_flightLocker)
            {
                return _context.Flights.Any(f =>
                    f.From.AirportName.ToLower().Trim() == request.From.AirportName.ToLower().Trim() &&
                    f.To.AirportName.ToLower().Trim() == request.To.AirportName.ToLower().Trim() &&
                    f.ArrivalTime == request.ArrivalTime &&
                    f.DepartureTime == request.DepartureTime);
            }
        }
    }
}
