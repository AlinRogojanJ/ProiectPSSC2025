﻿using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AutoMapper;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;
using Microsoft.Extensions.Configuration;
using ProiectPSSC2025.Services.Interfaces;
using ProiectPSSC2025.Models.DTOs;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IConfiguration _configuration;
    private readonly IBillingService _billingService;
    private readonly string _topicName = "room-reservation";

    public ReservationService(IReservationRepository repository,
                              IMapper mapper,
                              ServiceBusClient serviceBusClient,
                              IConfiguration configuration,
                              IBillingService billingService,
                              IRoomRepository roomRepository,
                              IUserRepository userRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
        _billingService = billingService;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
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
        Console.WriteLine(reservation);

        var isAvailable = (await _roomRepository.GetRoomByIdAsync(reservationDto.RoomId)).Status == "Available";

        if (!isAvailable)
        {
            throw new Exception("Room is not available.");
        }

        reservation.Status = "Pending";
        await _repository.AddAsync(reservation);

        if (reservation.Status == "Pending")
        {
            string queueName = _configuration["ServiceBus:Queues:RoomReservationQueue"];
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new InvalidOperationException("ServiceBus RoomReservationQueue is not configured.");
            }

            ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);

            string customText = $"Reservation with ID: {reservation.Id} for room {reservation.RoomId} was requested at {DateTime.UtcNow}.";

            var payload = new RoomReservationOutputDTO
            {
                ReservationId = reservation.Id,
                Status = reservation.Status

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
    }

    public async Task RemoveReservationAsync(string id)
    {
        await _repository.RemoveAsync(id);
    }

    public async Task UpdateReservationAsync(ReservationDTO reservationDto)
    {
        try
        {
            var newReservation = new Reservation()
            {
                UpdatedDate = DateTime.Now,
                StartDate = reservationDto.StartDate,
                EndDate = reservationDto.EndDate,
                RoomId = reservationDto.RoomId,
                Id = reservationDto.Id,
                UserId = reservationDto.UserId,
                CreatedDate = reservationDto.CreatedDate,
            };

            await _repository.UpdateAsync(newReservation);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task PublishReservationAsync(ReservationDTO reservationDTO)
    {
        try
        {
            var sender = _serviceBusClient.CreateSender(_topicName);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
