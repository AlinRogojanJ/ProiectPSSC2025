using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models.DTOs;
using ProiectPSSC2025.Services.Interfaces;
using ProiectPSSC2025.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Workfows
{
    public class BillingService : IBillingService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IConfiguration _configuration;
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUserRepository _userRepository;

        public BillingService(ServiceBusClient serviceBusClient, IConfiguration configuration, IRoomRepository roomRepository, IReservationRepository reservationRepository, IUserRepository userRepository)
        {
            _serviceBusClient = serviceBusClient;
            _configuration = configuration;
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
        }

        public async Task ProcessMessagesAsync(CancellationToken cancellationToken)
        {
            string queueName = _configuration["ServiceBus:Queues:RoomReservationQueue"];

            var processor = _serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync(cancellationToken);

            Console.WriteLine("Billing Service is processing messages.");

            // Wait for the background service to be canceled
            await Task.Delay(Timeout.Infinite, cancellationToken);

            // Stop processing when cancellation is requested
            await processor.StopProcessingAsync();
        }


        public async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                // Deserializare mesaj
                var messageBody = args.Message.Body.ToString();
                var roomReservationStatus = JsonSerializer.Deserialize<RoomReservationOutputDTO>(messageBody);

                var reservationDetails = await _reservationRepository.GetByIdAsync(roomReservationStatus.ReservationId);

                var room = await _roomRepository.GetRoomByIdAsync(reservationDetails.RoomId);
                var user = await _userRepository.GetUserByIdAsync(reservationDetails.UserId);

                int nights = (reservationDetails.EndDate - reservationDetails.StartDate).Days;
                if(nights <=0)
                {
                    nights = 1;
                }

                float totalCost = room.PricePerNight * nights;

                var paymentStatus = "Successful";

                if (user.Budget < totalCost)
                {
                    reservationDetails.Status = "Cancelled";
                    paymentStatus = "Failed";
                } else
                {
                    reservationDetails.Status = "Confirmed";
                    paymentStatus = "Successful";
                }

                await _reservationRepository.UpdateAsync(reservationDetails);

                // Confirmă procesarea mesajului
                await args.CompleteMessageAsync(args.Message);

                var responseMessage = new PaymentOutputDTO
                {
                    ReservationId = reservationDetails.Id,
                    PaymentStatus = paymentStatus,
                };

                await SendMessageAsync(responseMessage);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }


        public async Task SendMessageAsync(PaymentOutputDTO message)
        {
            string messageBody = JsonSerializer.Serialize(message);

            try
            {
                string statusQueue = _configuration["ServiceBus:Queues:BillingQueue"];
                ServiceBusSender sender = _serviceBusClient.CreateSender(statusQueue);

                ServiceBusMessage serviceBussMessage = new ServiceBusMessage(messageBody);
                await sender.SendMessageAsync(serviceBussMessage);
                Console.WriteLine($"Sent billing status message: {messageBody}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send billing status message: {messageBody}", ex);
            }

        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error processing message: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}
