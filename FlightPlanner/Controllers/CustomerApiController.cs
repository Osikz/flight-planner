using System.Linq;
using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [EnableCors]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _flightLocker = new object();

        public CustomerApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {
            lock (_flightLocker)
            {
                search = search.Trim().ToLower();

                var airports = _context.Airports.Where(f =>
                    f.AirportName.ToLower().Trim().Contains(search) ||
                    f.City.ToLower().Trim().Contains(search) ||
                    f.Country.ToLower().Trim().Contains(search));

                return Ok(airports);
            }
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchFlightsRequest req)
        {
            lock (_flightLocker)
            {
                var flights = _context.Flights.Where(f =>
                        f.From.AirportName.ToLower().Trim().Contains(req.From.ToLower().Trim()) ||
                        f.To.AirportName.ToLower().Trim().Contains(req.To.ToLower().Trim()) ||
                        f.DepartureTime == req.DepartureDate)
                    .Select(f => f);

                if (!FlightStorage.IsValidSearch(req))
                {
                    return BadRequest();
                }

                return Ok(SearchFlight(req, flights));
            }
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult FindFlightById(int id)
        {
            lock (_flightLocker)
            {
                var flight = _context.Flights
                    .Include(a => a.From)
                    .Include(a => a.To)
                    .SingleOrDefault(f => f.Id == id);

                if (flight == null)
                {
                    return NotFound();
                }

                return Ok(flight);
            }
        }

        private PageResult SearchFlight(SearchFlightsRequest request, IQueryable<Flight> flights)
        {
            lock (_flightLocker)
            {
                var pageResult = new PageResult();

                foreach (var flight in flights)
                {
                    if (flight == null)
                    {
                        return pageResult;
                    }

                    pageResult.Items.Add(flight);
                }

                pageResult.TotalItems = pageResult.Items.Count;
                pageResult.Page = pageResult.TotalItems;

                return pageResult;
            }
        }
    }
}
