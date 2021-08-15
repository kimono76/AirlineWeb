using System;
using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController,Route("api/[controller]")] public class WebhookSubscriptionController:ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public WebhookSubscriptionController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost] public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(WebhookSubscriptionCreateDto webhookSubscriptionCreateDto){
           var subscription = _context.WebhookSubscriptions.FirstOrDefault(x => x.WebhookURI == webhookSubscriptionCreateDto.WebhookURI);
           if (subscription == null){
               subscription= _mapper.Map<WebhookSubscription>(webhookSubscriptionCreateDto);
               subscription.Secret = Guid.NewGuid().ToString();
               subscription.WebhookPublisher = "PanAus";
               try
               {
                   _context.WebhookSubscriptions.Add(subscription);
                   _context.SaveChanges();
               } catch (Exception ex){
                   return BadRequest(ex.Message);
                }
                var webhookSubscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);
                return NoContent();//TODO replace with 201
           } else {
               return NoContent();
           }
        }
    }
}