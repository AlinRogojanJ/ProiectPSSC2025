using ProiectPSSC2025.Models.DTOs;
using ProiectPSSC2025.Models;
using ProiectPSSC2025.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using ProiectPSSC2025.DTOs;
using Microsoft.Extensions.Configuration;

namespace ProiectPSSC2025.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository,
                           IMapper mapper,
                           ServiceBusClient serviceBusClient,
                           IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _serviceBusClient = serviceBusClient;          
            _configuration = configuration;                
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<UserDTO?>(user);
        }

        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);

            // Service Bus

            string queueName = _configuration["ServiceBus:Queues:UserQueue"];
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new InvalidOperationException("ServiceBus UserQueue is not configured.");
            }

            ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);

            string customText = $"User {user.FirstName + user.LastName}  (ID: {user.Id}) was created at {DateTime.UtcNow}.";

            var payload = new
            {
                Message = customText,
                UserData = user,
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

    }
}
