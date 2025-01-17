using Azure.Messaging.ServiceBus;
using ProiectPSSC2025.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Interfaces
{
    public interface IRoomReservationService
    {
        Task ProcessMessagesAsync(CancellationToken cancellationToken);
        Task MessageHandler(ProcessMessageEventArgs args);
        Task SendMessageAsync(RoomReservationOutputDTO args);
        Task ErrorHandler(ProcessErrorEventArgs args);
    }
}
