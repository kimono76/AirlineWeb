using AirlineWeb.Dtos;

namespace AirlineWeb.MessageBus
{
    public interface IntMessageBusClient
    {
       void SendMessage(NotificationMessageDto notificationMessageDto); 
    }
}