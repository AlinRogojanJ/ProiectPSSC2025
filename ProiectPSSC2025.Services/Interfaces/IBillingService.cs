using Azure.Messaging.ServiceBus;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Interfaces
{
    public interface IBillingService
    {
        Task ProcessMessagesAsync(CancellationToken cancellationToken);
        Task MessageHandler(ProcessMessageEventArgs args);
        Task SendMessageAsync(PaymentOutputDTO args);
        Task ErrorHandler(ProcessErrorEventArgs args);
    }
}
