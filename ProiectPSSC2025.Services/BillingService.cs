using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Services.Interfaces;

public class BillingService : IBillingService
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IConfiguration _configuration;

    public BillingService(ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
    }

    public async Task ProcessBillingAsync(ReservationDTO reservationDto)
    {
        string queueName = _configuration["ServiceBus:QueueName"];
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException("ServiceBus QueueName is not configured.");
        }

        ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);

        await SendBillingMessageAsync( "Billing process started", sender, reservationDto);
        await Task.Delay(TimeSpan.FromSeconds(3));

        await SendBillingMessageAsync($"Generating the bill with the info for room {reservationDto.RoomId}", sender, reservationDto);
        await Task.Delay(TimeSpan.FromSeconds(5));

        await SendBillingMessageAsync("Bill generated", sender, reservationDto);
    }

    private async Task SendBillingMessageAsync(string statusMessage, ServiceBusSender sender, ReservationDTO reservationDto)
    {
        var payload = new
        {
            ReservationId = reservationDto.Id,
            RoomId = reservationDto.RoomId,
            StatusMessage = statusMessage,
            Timestamp = DateTime.UtcNow
        };

        string messageBody = JsonSerializer.Serialize(payload);
        ServiceBusMessage message = new ServiceBusMessage(messageBody);

        try
        {
            await sender.SendMessageAsync(message);
            Console.WriteLine($"Sent message: {statusMessage}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to send billing message: {statusMessage}", ex);
        }
    }
}
