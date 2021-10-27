using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CongestionFeeApp.Models
{
    public class TimeSplitCust
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DayOfWeek WeekDay { get; set; }

        public override string ToString()
        {
            return $"{StartTime}, {EndTime}, {WeekDay}";
        }
    }
}
