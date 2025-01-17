using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Models.Utils
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
