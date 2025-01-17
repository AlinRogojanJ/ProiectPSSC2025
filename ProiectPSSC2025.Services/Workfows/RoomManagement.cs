using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
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
    public class RoomManagement : IRoomManagement
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IConfiguration _configuration;
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;

        public RoomManagement(ServiceBusClient serviceBusClient, IConfiguration configuration, IRoomRepository roomRepository, IReservationRepository reservationRepository)
        {
            _serviceBusClient = serviceBusClient;
            _configuration = configuration;
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task ProcessMessagesAsync(CancellationToken cancellationToken)
        {
            string queueName = _configuration["ServiceBus:Queues:BillingQueue"];

            var processor = _serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += MessageHandler;

            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync(cancellationToken);

            Console.WriteLine("Room Management Service is processing messages. Press any key to stop...");
            Console.ReadKey();

            await processor.StopProcessingAsync();
        }

        public async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                // Deserializare mesaj
                var messageBody = args.Message.Body.ToString();
                var paymentStatus = JsonSerializer.Deserialize<PaymentOutputDTO>(messageBody);
                var reservationDetails = await _reservationRepository.GetByIdAsync(paymentStatus.ReservationId);
                var roomStatus = "Reserved";
                // Procesare mesaj
                if (paymentStatus.PaymentStatus == "Successful")
                {
                    Console.WriteLine($"Reservation {paymentStatus.ReservationId} is paid. Updating room status...");

                    // Actualizează starea camerei
                    roomStatus = "Reserved";
                }
                else
                {
                    Console.WriteLine($"Reservation {paymentStatus.ReservationId} is not paid. Updating room status...");

                    // Actualizează starea camerei
                    roomStatus = "Available";
                }

                await _roomRepository.UpdateRoomStatusAsync(reservationDetails.RoomId, roomStatus);

                // Confirmă procesarea mesajului
                await args.CompleteMessageAsync(args.Message);

                var responseMessage = new RoomManagementResponseDTO
                {
                    ReservationId = reservationDetails.Id,
                    RoomId = reservationDetails.RoomId,
                    RoomStatus = roomStatus,
                };

                await SendRoomStatusMessageAsync(responseMessage);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        public async Task SendRoomStatusMessageAsync(RoomManagementResponseDTO message)
        {
            string messageBody = JsonSerializer.Serialize(message);
            
            try
            {
                string roomStatusQueue = _configuration["ServiceBus:Queues:RoomStatusQueue"];
                ServiceBusSender sender = _serviceBusClient.CreateSender(roomStatusQueue);

                ServiceBusMessage serviceBussMessage = new ServiceBusMessage(messageBody);
                await sender.SendMessageAsync(serviceBussMessage);
                Console.WriteLine($"Sent room status message: {messageBody}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send room status message: {messageBody}", ex);
            }

        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error processing message: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}
