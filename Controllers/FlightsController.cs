using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class FlightsController: ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public FlightsController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{flightCode}", Name="GetFlightDetailsByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightDetailsByCode(string flightCode){
            var flight = _context.FlightDetails.FirstOrDefault(x=>x.FlightCode == flightCode);
            if (flight is null) return NotFound();
            return Ok(_mapper.Map<FlightDetailReadDto>(flight));
        }
    }
}