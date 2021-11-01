using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models
{
    public class Range
    {
        public int _id { get; set; }
        public string _alias { get; set; }
        public TimeSpan _start { get; set; }
        public TimeSpan _end { get; set; }
        public Dictionary<VehicleTypes, decimal> _feeList { get; set; }
    }
}
