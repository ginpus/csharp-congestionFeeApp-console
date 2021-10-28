using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CongestionFeeApp.Models
{
    public class Periods // all the periods for specific charge period
    {
        public TimeRangeCust TimeRange {get; set;}

        public List<TimeSpanCust> TimeSpans {  get; set;}
    }
}
