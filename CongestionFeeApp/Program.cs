using CongestionFeeApp.Contracts.Enums;
using CongestionFeeApp.Models;
//using Itenso.TimePeriod;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CongestionFeeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var timeRange = new TimeRangeCust
            {
                //StartTime = Ceiling(new DateTime (2021, 10, 21, 07, 37, 57), TimeSpan.FromSeconds(60)),
                //EndTime = Floor(new DateTime(2021, 10, 21, 13, 37, 57), TimeSpan.FromSeconds(60))

                //INPUT 1
                //Car: 24 / 04 / 2008 11:32 - 24 / 04 / 2008 14:42
                //OUTPUT 1
                //Charge for 0h 28m(AM rate): £0.90
                //Charge for 2h 42m(PM rate): £6.70
                //Total Charge: £7.60

                /*StartTime = new DateTime(2008, 4, 24, 11, 32, 0),
                EndTime = new DateTime(2008, 4, 24, 14, 42, 0)*/

                //INPUT 2
                //Motorbike: 24 / 04 / 2008 17:00 - 24 / 04 / 2008 22:11
                //OUTPUT 2
               // Charge for 0h 0m(AM rate): £0.00
               // Charge for 2h 0m(PM rate): £2.00
               // Total Charge: £2.00

                /*StartTime = new DateTime(2008, 4, 24, 17, 0, 0),
                EndTime = new DateTime(2008, 4, 24, 22, 11, 0)*/

                //INPUT 3
                //Van: 25 / 04 / 2008 10:23 - 28 / 04 / 2008 09:02
                //OUTPUT 3
                //Charge for 3h 39m(AM rate): £7.30
                //Charge for 7h 0m(PM rate): £17.50
               //Total Charge: £24.80

                StartTime = new DateTime(2008, 4, 25, 10, 23, 0),
                EndTime = new DateTime(2008, 4, 28, 09, 2, 0)
            };

            var amStartHour = 7; // get from DB and put into array
            var pmStartHour = 12;
            var pmEndHour = 19;

            var daySplits = Enumerable.Range(0, (timeRange.EndTime.Date - timeRange.StartTime.Date).Days + 1)
                  .Select(c => new TimeSplitCust
                  {
                      StartTime = Max(timeRange.StartTime.Date.AddDays(c), timeRange.StartTime),
                      EndTime = Min(timeRange.StartTime.Date.AddDays(c + 1).AddMilliseconds(-1), timeRange.EndTime),
                      WeekDay = Max(timeRange.StartTime.Date.AddDays(c), timeRange.StartTime).DayOfWeek                 
                  });

            var ranges = daySplits.ToList();

            var fullDayCharge = new List<TimeSplitCust> { };

            var amChargeDuration = new List<TimeSpan> { };

            var pmChargeDuration = new List<TimeSpan> { };

            for (var i = 0; i < ranges.Count; i++)
            {
                Console.WriteLine(ranges[i]);
                var amStartTime = new DateTime(ranges[i].StartTime.Year, ranges[i].StartTime.Month, ranges[i].StartTime.Day, amStartHour, 0, 0);
                var pmStartTime = new DateTime(ranges[i].EndTime.Year, ranges[i].EndTime.Month, ranges[i].EndTime.Day, pmStartHour, 0, 0);
                var pmEndTime = new DateTime(ranges[i].EndTime.Year, ranges[i].EndTime.Month, ranges[i].EndTime.Day, pmEndHour, 0, 0);

                if (ranges[i].StartTime <= amStartTime  && 
                    ranges[i].EndTime >= pmEndTime && 
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S0; P3");
                    fullDayCharge.Add(ranges[i]);
                    var amSlot = pmStartTime - amStartTime;
                    var pmSlot = pmEndTime - pmStartTime;

                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (ranges[i].StartTime > amStartTime &&
                    ranges[i].StartTime < pmStartTime &&
                    ranges[i].EndTime >= pmEndTime &&
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S1; P3");
                    var amSlot = pmStartTime - ranges[i].StartTime;
                    var pmSlot = pmEndTime - pmStartTime;
                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (ranges[i].StartTime >= pmStartTime &&
                    ranges[i].StartTime < pmEndTime &&
                    ranges[i].EndTime >= pmEndTime &&
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S2; P3");
                    var pmSlot = pmEndTime - ranges[i].StartTime;
                    pmChargeDuration.Add(pmSlot);
                }

                if (ranges[i].StartTime >= pmStartTime &&
                    ranges[i].StartTime < pmEndTime &&
                    ranges[i].EndTime < pmEndTime &&
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S2; P2");
                    var pmSlot = ranges[i].EndTime - ranges[i].StartTime;
                    pmChargeDuration.Add(pmSlot);
                }

                if (ranges[i].StartTime > amStartTime &&
                    ranges[i].StartTime < pmStartTime &&
                    ranges[i].EndTime < pmEndTime &&
                    ranges[i].EndTime > pmStartTime &&
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S1; P2");
                    var amSlot = pmStartTime - ranges[i].StartTime;
                    var pmSlot = ranges[i].EndTime - pmStartTime;
                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (ranges[i].StartTime >= amStartTime &&
                    ranges[i].StartTime < pmStartTime &&
                    ranges[i].EndTime <= pmStartTime &&
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S1; P1");
                    var amSlot = ranges[i].EndTime - ranges[i].StartTime;
                    amChargeDuration.Add(amSlot);
                }

                if (ranges[i].StartTime <= amStartTime &&
                    ranges[i].EndTime < pmEndTime &&
                    ranges[i].EndTime > pmStartTime &&
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S0; P1");
                    var amSlot = pmStartTime - amStartTime;
                    var pmSlot = ranges[i].EndTime - pmStartTime;
                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (ranges[i].StartTime <= amStartTime &&
                    ranges[i].EndTime <= pmStartTime &&
                    IsChargeableDay((int)ranges[i].WeekDay))
                {
                    Console.WriteLine("S0; P1");
                    var amSlot = ranges[i].EndTime - amStartTime;
                    amChargeDuration.Add(amSlot);
                }

                var totalAmSpan = new TimeSpan(amChargeDuration.Sum(r => r.Ticks));
                var totalPmSpan = new TimeSpan(pmChargeDuration.Sum(r => r.Ticks));

                Console.WriteLine($"Charge for {Math.Floor(totalAmSpan.TotalHours)}h {totalAmSpan.Minutes}m (AM rate)");
                Console.WriteLine($"Charge for {Math.Floor(totalPmSpan.TotalHours)}h {totalPmSpan.Minutes}m (PM rate)");
            }

            Console.WriteLine();

            for (var i = 0; i < amChargeDuration.Count; i++)
            {
                Console.WriteLine(amChargeDuration[i]);
            }

            /*DateTime chunkEnd;
            while ((chunkEnd = timeRange.StartTime.AddMinutes(7)) < timeRange.EndTime)
            {
                yield return Tuple.Create(timeRange.StartTime, chunkEnd, timeRange.StartTime.DayOfWeek);
                timeRange.StartTime = chunkEnd;
            }
            yield return Tuple.Create(timeRange.StartTime, timeRange.EndTime, timeRange.StartTime.DayOfWeek);*/
        }

/*        public bool IsValidReservation(DateTime start, DateTime end)
        {
            if (!TimeCompare.IsSameDay(start, end))
            {
                return false;  // multiple day reservation
            }

            TimeRange workingHours =
              new TimeRange(TimeTrim.Hour(start, 8), TimeTrim.Hour(start, 18));
            return workingHours.HasInside(new TimeRange(start, end));
        }*/

        public static DateTime Floor(DateTime dateTime, TimeSpan interval)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }

        public static DateTime Ceiling(DateTime dateTime, TimeSpan interval)
        {
            var overflow = dateTime.Ticks % interval.Ticks;

            return overflow == 0 ? dateTime : dateTime.AddTicks(interval.Ticks - overflow);
        }

        private static DateTime Min(DateTime a, DateTime b)
        {
            if (a > b)
                return b;
            return a;
        }

        private static DateTime Max(DateTime a, DateTime b)
        {
            if (a < b)
                return b;
            return a;
        }

        private static bool IsChargeableDay(int dayOfWeek)
        {
           return Enum.IsDefined(typeof(ChargeableDays), dayOfWeek);
        }
    }

}
