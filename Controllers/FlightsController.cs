using System;
using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.MessageBus;
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
        private readonly IntMessageBusClient _messageBusClient;

        public FlightsController(AirlineDbContext context, IMapper mapper, IntMessageBusClient messageBusClient)
        {
            _context = context;
            _mapper = mapper;
            _messageBusClient= messageBusClient;
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
        [HttpPut("{id}")]
        public ActionResult UpdateFlightDetail(int id,FlightDetailUpdateDto flightDetailUpdateDto){
            var flight= _context.FlightDetails.FirstOrDefault(x=>x.Id == id);
            if (flight is null) return NotFound();
            decimal oldPrice= flight.Price;
            _mapper.Map(flightDetailUpdateDto, flight);
            try
            {
                _context.SaveChanges();
                if (oldPrice != flight.Price){
                    Console.WriteLine("PRICE CHANGED - Place message on BUS");
                    var notificationMessageDto= new NotificationMessageDto{
                        WebhookType="pricechange",
                        OldPrice= oldPrice,
                        NewPrice=flight.Price,
                        FlightCode=flight.FlightCode,
                    };
                    _messageBusClient.SendMessage(notificationMessageDto);
                    } else Console.WriteLine("NO price change");
                return NoContent(); 
            } catch (System.Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}