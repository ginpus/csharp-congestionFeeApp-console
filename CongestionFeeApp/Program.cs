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
            var range = new TimeRangeCust
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

                Start = new DateTime(2008, 4, 25, 10, 23, 0),
                End = new DateTime(2008, 4, 30, 11, 2, 0)
            };

            if(range.Start > range.End)
            {
                throw new Exception("Invalid time expression - end time cannot be earlier than start time");
            }

            var amStartHour = 7; // get from DB and put into array. Or from appsettings
            var pmStartHour = 12;
            var pmEndHour = 19;

            var amStart = new TimeSpan(amStartHour, 0, 0);
            var pmStart = new TimeSpan(pmStartHour, 0, 0);
            var pmEnd = new TimeSpan(pmEndHour, 0, 0);

            var chargeThresholds = new List<TimeSpan> {
                amStart,
                pmStart,  
                pmEnd};


            var chargeDays = Enumerable.Range(0, (range.End.Date - range.Start.Date).Days + 1)
                  .Select(d => new TimeSplitCust
                  {
                      StartTime = Max(range.Start.Date.AddDays(d), range.Start),
                      EndTime = Min(range.Start.Date.AddDays(d + 1).AddMilliseconds(-1), range.End),
                      WeekDay = Max(range.Start.Date.AddDays(d), range.Start).DayOfWeek,
                      Thresholds = new List<TimeSpan> { }
                  })
                  .Where(d => IsChargeableDay((int)d.WeekDay))
                  .ToList();

            // for each range/period, that has charge > 0, create new List<TimeSpan> {}

            for (var i = 0; i < chargeDays.Count; i++)
            {
                for (var j = 0; j< chargeThresholds.Count; j++) 
                { 
                    chargeDays[i].Thresholds.Add(chargeThresholds[j]);
                }
                if (i == 0)
                {
                    chargeDays[i].Thresholds
                        .RemoveAll(d => d <= chargeDays[i].StartTime.TimeOfDay);
                    chargeDays[i].Thresholds.Add(chargeDays[i].StartTime.TimeOfDay);
                }
                if (i == chargeDays.Count - 1)
                {
                    chargeDays[i].Thresholds
                        .RemoveAll(d => d >= chargeDays[i].EndTime.TimeOfDay);
                    chargeDays[i].Thresholds.Add(chargeDays[i].EndTime.TimeOfDay);
                }
                chargeDays[i].Thresholds = chargeDays[i].Thresholds
                    .OrderBy(t => t.TotalMinutes).ToList();
            }

            
            var amChargeDuration = new List<TimeSpan> { };
            var pmChargeDuration = new List<TimeSpan> { };

            for (var i = 0; i < chargeDays.Count; i++)
            {
                Console.WriteLine(chargeDays[i]);

                if (chargeDays[i].StartTime.TimeOfDay <= amStart  && 
                    chargeDays[i].EndTime.TimeOfDay >= pmEnd)
                {
                    Console.WriteLine("S0; P3");
                    var amSlot = pmStart - amStart;
                    var pmSlot = pmEnd - pmStart;

                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (chargeDays[i].StartTime.TimeOfDay > amStart &&
                    chargeDays[i].StartTime.TimeOfDay < pmStart &&
                    chargeDays[i].EndTime.TimeOfDay >= pmEnd)
                {
                    Console.WriteLine("S1; P3");
                    var amSlot = pmStart - chargeDays[i].StartTime.TimeOfDay;
                    var pmSlot = pmEnd - pmStart;
                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (chargeDays[i].StartTime.TimeOfDay >= pmStart &&
                    chargeDays[i].StartTime.TimeOfDay < pmEnd &&
                    chargeDays[i].EndTime.TimeOfDay >= pmEnd)
                {
                    Console.WriteLine("S2; P3");
                    var pmSlot = pmEnd - chargeDays[i].StartTime.TimeOfDay;
                    pmChargeDuration.Add(pmSlot);
                }

                if (chargeDays[i].StartTime.TimeOfDay >= pmStart &&
                    chargeDays[i].StartTime.TimeOfDay < pmEnd &&
                    chargeDays[i].EndTime.TimeOfDay < pmEnd)
                {
                    Console.WriteLine("S2; P2");
                    var pmSlot = chargeDays[i].EndTime.TimeOfDay - chargeDays[i].StartTime.TimeOfDay;
                    pmChargeDuration.Add(pmSlot);
                }

                if (chargeDays[i].StartTime.TimeOfDay > amStart &&
                    chargeDays[i].StartTime.TimeOfDay < pmStart &&
                    chargeDays[i].EndTime.TimeOfDay < pmEnd &&
                    chargeDays[i].EndTime.TimeOfDay > pmStart)
                {
                    Console.WriteLine("S1; P2");
                    var amSlot = pmStart - chargeDays[i].StartTime.TimeOfDay;
                    var pmSlot = chargeDays[i].EndTime.TimeOfDay - pmStart;
                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (chargeDays[i].StartTime.TimeOfDay >= amStart &&
                    chargeDays[i].StartTime.TimeOfDay < pmStart &&
                    chargeDays[i].EndTime.TimeOfDay <= pmStart)
                {
                    Console.WriteLine("S1; P1");
                    var amSlot = chargeDays[i].EndTime.TimeOfDay - chargeDays[i].StartTime.TimeOfDay;
                    amChargeDuration.Add(amSlot);
                }

                if (chargeDays[i].StartTime.TimeOfDay <= amStart &&
                    chargeDays[i].EndTime.TimeOfDay < pmEnd &&
                    chargeDays[i].EndTime.TimeOfDay > pmStart)
                {
                    Console.WriteLine("S0; P1");
                    var amSlot = pmStart - amStart;
                    var pmSlot = chargeDays[i].EndTime.TimeOfDay - pmStart;
                    amChargeDuration.Add(amSlot);
                    pmChargeDuration.Add(pmSlot);
                }

                if (chargeDays[i].StartTime.TimeOfDay <= amStart &&
                    chargeDays[i].EndTime.TimeOfDay <= pmStart)
                {
                    Console.WriteLine("S0; P1");
                    var amSlot = chargeDays[i].EndTime.TimeOfDay - amStart;
                    amChargeDuration.Add(amSlot);
                }

                var totalAmSpan = new TimeSpan(amChargeDuration.Sum(r => r.Ticks));
                var totalPmSpan = new TimeSpan(pmChargeDuration.Sum(r => r.Ticks));

                Console.WriteLine($"Charge for {Math.Floor(totalAmSpan.TotalHours)}h {totalAmSpan.Minutes}m (AM rate)");
                Console.WriteLine($"Charge for {Math.Floor(totalPmSpan.TotalHours)}h {totalPmSpan.Minutes}m (PM rate)");
            }
        }

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
