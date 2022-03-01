using FlightPlanner.Storage;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("testing-api")]
    [EnableCors]
    [ApiController]
    public class TestingApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;

        public TestingApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("clear")]
        public IActionResult Clear()
        {
            _context.Flights.RemoveRange(_context.Flights);
            _context.Airports.RemoveRange(_context.Airports);
            _context.SaveChanges();

            return Ok();
        }
    }
}
