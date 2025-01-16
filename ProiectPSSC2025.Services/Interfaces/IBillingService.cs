using ProiectPSSC2025.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Interfaces
{
    public interface IBillingService
    {
        Task ProcessBillingAsync(ReservationDTO reservationDto);
    }
}
