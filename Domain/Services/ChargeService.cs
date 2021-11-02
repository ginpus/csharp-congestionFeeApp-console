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
        private List<TimeSplit> _chargeDays { get; set; }
        private List<TimeSpan> _periodThresholds { get; set; }

        public ChargeService(IChargeRepository chargeRepository)
        {
            _chargeRepository = chargeRepository;
        }
        
        public Dictionary<TimeSpan, double> CalculateChargePeriods(TimeRange range)
        {
            if (range.Start > range.End)
            {
                throw new Exception("Invalid time expression - end time cannot be earlier than start time");
            }

            _chargeDays = Enumerable.Range(0, (range.End.Date - range.Start.Date).Days + 1)
              .Select(d => new TimeSplit
              {
                  StartTime = Max(range.Start.Date.AddDays(d), range.Start),
                  EndTime = Min(range.Start.Date.AddDays(d + 1).AddMilliseconds(-1), range.End),
                  WeekDay = Max(range.Start.Date.AddDays(d), range.Start).DayOfWeek,
                  Thresholds = new List<TimeSpan> { }
              })
              .Where(d => IsChargeableDay((int)d.WeekDay))
              .ToList();

            _periodThresholds = _chargeRepository.GetPeriodThresholds();

            for (var i = 0; i < _chargeDays.Count; i++)
            {
                for (var j = 0; j < _periodThresholds.Count; j++)
                {
                    _chargeDays[i].Thresholds.Add(_periodThresholds[j]);
                }
                if (i == 0)
                {
                    _chargeDays[i].Thresholds
                        .RemoveAll(d => d <= _chargeDays[i].StartTime.TimeOfDay);
                    _chargeDays[i].Thresholds.Add(_chargeDays[i].StartTime.TimeOfDay);
                }
                if (i == _chargeDays.Count - 1)
                {
                    _chargeDays[i].Thresholds
                        .RemoveAll(d => d >= _chargeDays[i].EndTime.TimeOfDay);
                    _chargeDays[i].Thresholds.Add(_chargeDays[i].EndTime.TimeOfDay);
                }
                _chargeDays[i].Thresholds = _chargeDays[i].Thresholds
                    .OrderBy(t => t.TotalMinutes).ToList();
            }

            var splitDuration = new List<List<TimeSpan>>();
            var totalDuration = new List<TimeSpan>();
            var periodDurations = new Dictionary<TimeSpan, double>();

            for (var k = 0; k < _periodThresholds.Count - 1; k++)
            {
                splitDuration.Add(new List<TimeSpan>());
                for (var i = 0; i < _chargeDays.Count; i++)
                {
                    for (var l = 0; l < _chargeDays[i].Thresholds.Count - 1; l++)
                    {
                        if (_chargeDays[i].Thresholds[l].TotalMinutes >= _periodThresholds[k].TotalMinutes &&
                           _chargeDays[i].Thresholds[l].TotalMinutes < _periodThresholds[k + 1].TotalMinutes)
                        {
                            var chargeSplitDuration = _chargeDays[i].Thresholds[l + 1] - _chargeDays[i].Thresholds[l];
                            splitDuration[k].Add(chargeSplitDuration);
                        }
                    }
                }
                totalDuration.Add(new TimeSpan(splitDuration[k].Sum(r => r.Ticks)));
                var totalHours = Math.Floor(totalDuration[k].TotalHours);
                var minutes = totalDuration[k].Minutes;
                Console.WriteLine($"Charge for {totalHours}h {minutes}m (rate {k + 1})");

                periodDurations.Add(_periodThresholds[k], totalDuration[k].TotalMinutes);
            }

            return periodDurations;
        }

        public List<double> CalculateCharges(Dictionary<TimeSpan, double> totalDurations, VehicleTypes type)
        {
            var results = new List<double>();
            foreach(var entry in totalDurations)
            {
                var multiplier = _chargeRepository.GetRates(entry.Key ,type);
                var sum = multiplier * entry.Value / 60;
                results.Add(sum);
            }
            return results;
        }

        public void GetDefaultChargeValues()
        {
            var chargeThresholds = _chargeRepository.GetPeriodThresholds();

            foreach (var chargeThreshold in chargeThresholds)
            {
                Console.WriteLine(chargeThreshold);
            }
        }

        public void PrintVechicleTypes()
        {
            var count = 0;
            foreach (var name in Enum.GetNames(typeof(VehicleTypes)))
            {
                Console.WriteLine($"{++count} - {name}");
            }
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