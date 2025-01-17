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
        string queueName = _configuration["ServiceBus:Queues:BillingQueue"];
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException("ServiceBus BillingQueue is not configured.");
        }

        ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);

        await SendBillingMessageAsync("Bill generated", "Successful", sender, reservationDto);
    }

    private async Task SendBillingMessageAsync(string statusMessage, string paymentStatus, ServiceBusSender sender, ReservationDTO reservationDto)
    {
        var payload = new
        {
            PaymentStatus = paymentStatus,
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
            Console.WriteLine($"Sent billing message: {statusMessage} with PaymentStatus: {paymentStatus}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to send billing message: {statusMessage}", ex);
        }
    }
}
