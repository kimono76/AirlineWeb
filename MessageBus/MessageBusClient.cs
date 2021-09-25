using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AirlineWeb.Dtos;
using RabbitMQ.Client;

namespace AirlineWeb.MessageBus
{
    public class MessageBusClient : IntMessageBusClient
    {
        public void SendMessage(NotificationMessageDto notificationMessageDto)
        {
            var factory = new ConnectionFactory(){
                HostName="localhost",
                Port = 5672,
            };
            using (var connection = factory.CreateConnection())
            using (var channel= connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange:"trigger",
                    type: ExchangeType.Fanout
                    );
                var message = JsonSerializer.Serialize(notificationMessageDto);
                var body= Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange:"trigger",
                    routingKey:"",
                    basicProperties:null,
                    body:body
                ); 
                Console.WriteLine("=> Message published on Message Bus");
            }
        }
    }
}