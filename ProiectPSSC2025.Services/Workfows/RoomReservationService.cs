using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;
using ProiectPSSC2025.Models.DTOs;
using ProiectPSSC2025.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Workfows
{
    public class RoomReservationService : IRoomReservationService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IConfiguration _configuration;
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUserRepository _userRepository;

        public RoomReservationService(ServiceBusClient serviceBusClient, IConfiguration configuration, IRoomRepository roomRepository, IReservationRepository reservationRepository, IUserRepository userRepository)
        {

            _serviceBusClient = serviceBusClient;
            _configuration = configuration;
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;

        }

        public async Task ProcessMessagesAsync(CancellationToken cancellationToken)
        {
            string queueName = _configuration["ServiceBus:Queues:RoomReservationRequestQueue"];
            var processor = _serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync(cancellationToken);

            Console.WriteLine("Room Reservation Service is processing messages.");

            // Wait for the background service to be canceled
            await Task.Delay(Timeout.Infinite, cancellationToken);

            // Stop processing when cancellation is requested
            await processor.StopProcessingAsync();
        }

        public async Task MessageHandler(ProcessMessageEventArgs args)
        {
            // Deserializare mesaj
            var messageBody = args.Message.Body.ToString();
            var roomReservationRequest = JsonSerializer.Deserialize<RoomReservationRequestDTO>(messageBody);
            var reservationStatus = "Pending";

            var isAvailable = (await _roomRepository.GetRoomByIdAsync(roomReservationRequest.RoomId)).Status == "Available";
            var user = await _userRepository.GetUserByIdAsync(roomReservationRequest.UserId);

            if (!isAvailable || user == null)
            {
                reservationStatus = "Cancelled";
                throw new Exception("Room not available / User not found");
            }

            var reservation = new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                StartDate = roomReservationRequest.StartDate,
                EndDate = roomReservationRequest.EndDate,
                RoomId = roomReservationRequest.RoomId,
                UserId = roomReservationRequest.UserId,
                Status = reservationStatus
            };

            await _reservationRepository.AddAsync(reservation);

            // Confirmă procesarea mesajului
            await args.CompleteMessageAsync(args.Message);

            var responseMessage = new RoomReservationOutputDTO
            {
                Status = reservationStatus,
                ReservationId = reservation.Id,
            };

            await SendMessageAsync(responseMessage);
        }

        public async Task SendMessageAsync(RoomReservationOutputDTO message)
        {
            string messageBody = JsonSerializer.Serialize(message);

            try
            {
                string statusQueue = _configuration["ServiceBus:Queues:RoomReservationQueue"];
                ServiceBusSender sender = _serviceBusClient.CreateSender(statusQueue);

                ServiceBusMessage serviceBussMessage = new ServiceBusMessage(messageBody);
                await sender.SendMessageAsync(serviceBussMessage);
                Console.WriteLine($"Sent room reservation status message: {messageBody}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send room reservation status message: {messageBody}", ex);
            }
        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error processing message: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}
