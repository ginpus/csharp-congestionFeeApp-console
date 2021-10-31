using Contracts.Enums;
using Domain.Models;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    public class ChargeService : IChargeService
    {
        private readonly IChargeRepository _chargeRepository;

        public ChargeService(IChargeRepository chargeRepository)
        {
            _chargeRepository = chargeRepository;
        }

        public List<TimeSplit> SplitChargableDays(TimeRange range)
        {
            if (range.Start > range.End)
            {
                throw new Exception("Invalid time expression - end time cannot be earlier than start time");
            }

            var chargeDays = Enumerable.Range(0, (range.End.Date - range.Start.Date).Days + 1)
              .Select(d => new TimeSplit
              {
                  StartTime = Max(range.Start.Date.AddDays(d), range.Start),
                  EndTime = Min(range.Start.Date.AddDays(d + 1).AddMilliseconds(-1), range.End),
                  WeekDay = Max(range.Start.Date.AddDays(d), range.Start).DayOfWeek,
                  Thresholds = new List<TimeSpan> { }
              })
              .Where(d => IsChargeableDay((int)d.WeekDay))
              .ToList();

            var chargeThresholds = _chargeRepository.GetChargeThresholds();

            for (var i = 0; i < chargeDays.Count; i++)
            {
                for (var j = 0; j < chargeThresholds.Count; j++)
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

            var splitDuration = new List<List<TimeSpan>>();
            var totalDuration = new List<TimeSpan>();

            for (var k = 0; k < chargeThresholds.Count - 1; k++)
            {
                splitDuration.Add(new List<TimeSpan>());
                for (var i = 0; i < chargeDays.Count; i++)
                {
                    for (var l = 0; l < chargeDays[i].Thresholds.Count - 1; l++)
                    {
                        if (chargeDays[i].Thresholds[l].TotalMinutes >= chargeThresholds[k].TotalMinutes &&
                           chargeDays[i].Thresholds[l].TotalMinutes < chargeThresholds[k + 1].TotalMinutes)
                        {
                            var chargeSplitDuration = chargeDays[i].Thresholds[l + 1] - chargeDays[i].Thresholds[l];
                            splitDuration[k].Add(chargeSplitDuration);
                        }
                    }
                }
                totalDuration.Add(new TimeSpan(splitDuration[k].Sum(r => r.Ticks)));
                Console.WriteLine($"Charge for {Math.Floor(totalDuration[k].TotalHours)}h {totalDuration[k].Minutes}m (rate {k + 1})");
            }

            return chargeDays;
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