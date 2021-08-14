using AirlineWeb.Data;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController,Route("api/[controller]")]
    public class WebhookSubscriptionController:ControllerBase
    {
        private readonly AirlineDbContext _context;

        public WebhookSubscriptionController(AirlineDbContext context)
        {
            _context = context;
        }
    }
}