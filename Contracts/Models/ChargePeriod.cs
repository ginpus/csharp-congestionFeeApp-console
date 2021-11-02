using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models
{
    public class ChargePeriod
    {
        public Guid Id { get; set; }
        public string Alias { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public Dictionary<VehicleTypes, double> FeeList { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }
}
