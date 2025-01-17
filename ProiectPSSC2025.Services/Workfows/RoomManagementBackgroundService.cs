using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProiectPSSC2025.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Workfows
{
    public class RoomManagementBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RoomManagementBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var roomManagement = scope.ServiceProvider.GetRequiredService<IRoomManagement>();
                        var billingService = scope.ServiceProvider.GetRequiredService<IBillingService>();

                        var roomManagementTask = roomManagement.ProcessMessagesAsync(stoppingToken);
                        var billingServiceTask = billingService.ProcessMessagesAsync(stoppingToken);

                        await Task.WhenAll(roomManagementTask, billingServiceTask);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in background service: {ex.Message}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
