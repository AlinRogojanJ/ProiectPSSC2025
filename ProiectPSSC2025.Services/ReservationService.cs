using System;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using AutoMapper;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;
using Microsoft.Extensions.Configuration;
using ProiectPSSC2025.Services.Interfaces;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IMapper _mapper;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IConfiguration _configuration;
    private readonly IBillingService _billingService;

    public ReservationService(IReservationRepository repository,
                              IMapper mapper,
                              ServiceBusClient serviceBusClient,
                              IConfiguration configuration,
                              IBillingService billingService)
    {
        _repository = repository;
        _mapper = mapper;
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
        _billingService = billingService;
    }

    public async Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync()
    {
        var reservations = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
    }

    public async Task<ReservationDTO> GetReservationByIdAsync(string id)
    {
        var reservation = await _repository.GetByIdAsync(id);
        return _mapper.Map<ReservationDTO>(reservation);
    }

    public async Task CreateReservationAsync(ReservationDTO reservationDto)
    {
        var reservation = _mapper.Map<Reservation>(reservationDto);
        await _repository.AddAsync(reservation);

        if (reservation.Status == "Pending")
        {
            string queueName = _configuration["ServiceBus:Queues:RoomReservationQueue"];
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new InvalidOperationException("ServiceBus RoomReservationQueue is not configured.");
            }

            ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);

            string customText = $"Reservation with ID: {reservation.Id} for room {reservation.RoomId} was booked at {DateTime.UtcNow}.";

            var payload = new
            {
                Message = customText,
                UserData = reservation,
                Timestamp = DateTime.UtcNow
            };

            string messageBody = JsonSerializer.Serialize(payload);
            ServiceBusMessage message = new ServiceBusMessage(messageBody);

            try
            {
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send message to Service Bus.", ex);
            }
        }

        // Start the billing workflow.
        await _billingService.ProcessBillingAsync(reservationDto);
    }
}
