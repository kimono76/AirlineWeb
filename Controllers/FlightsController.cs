using System;
using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
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
        [HttpPost]
        public ActionResult<FlightDetailReadDto> CreateFlight(FlightDetailCreateDto flightDetailCreateDto){
            var flight = _context.FlightDetails.FirstOrDefault(x=>x.FlightCode == flightDetailCreateDto.FlightCode);
            if (flight is null){
                var flightDetailModel = _mapper.Map<FlightDetail>(flightDetailCreateDto);
                try{
                    _context.FlightDetails.Add(flightDetailModel);
                    _context.SaveChanges();
                } catch (Exception ex){ return BadRequest(ex.Message);}
                var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetailModel);
                return CreatedAtRoute(
                    nameof(this.GetFlightDetailsByCode), 
                    new{flightCode= flightDetailReadDto.FlightCode},flightDetailReadDto);
            } else return NoContent();
        }
    }
}