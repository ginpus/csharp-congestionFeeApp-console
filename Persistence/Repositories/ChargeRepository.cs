using Contracts.Enums;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Persistence.Repositories
{
    public class ChargeRepository : IChargeRepository
    {
        private readonly List<TimeSpan> _periodThresholds = new List<TimeSpan>();
        private readonly List<ChargePeriodMain> _periods = new List<ChargePeriodMain>();

        protected string _fileName = "ranges_json.txt";

        public ChargePeriodMain _newPeriod { get; set; }

        public string[] _savedPeriods { get; set; }

        public ChargeRepository()
        {
            var amPeriod = new ChargePeriodMain
            {
                Id = Guid.NewGuid(),
                Alias = "AM rate",
                Start = new TimeSpan(7, 0, 0),
                End = new TimeSpan(12, 0, 0),
                FeeList = new Dictionary<VehicleTypes, double>()
            };

            amPeriod.FeeList.Add(VehicleTypes.Car, 2.00);
            amPeriod.FeeList.Add(VehicleTypes.Motorbike, 1.00);

            _periods.Add(amPeriod);

            var pmPeriod = new ChargePeriodMain
            {
                Id = Guid.NewGuid(),
                Alias = "PM rate",
                Start = new TimeSpan(12, 0, 0),
                End = new TimeSpan(19, 0, 0),
                FeeList = new Dictionary<VehicleTypes, double>()
            };

            pmPeriod.FeeList.Add(VehicleTypes.Car, 2.50);
            pmPeriod.FeeList.Add(VehicleTypes.Motorbike, 1.00);

            _periods.Add(pmPeriod);

            var allPeriodThresholds = new List<TimeSpan> { };

            foreach (var range in _periods)
            {
                allPeriodThresholds.Add(range.Start);
                allPeriodThresholds.Add(range.End);
            }

            var periodThresholds = allPeriodThresholds
                .GroupBy(p => p)
                .Select(g => g.First())
                .ToList();

            _periodThresholds = periodThresholds;
        }

        public double GetRates(TimeSpan startOfPeriod, VehicleTypes type)
        {
            double rate;

            _periods
                 .FirstOrDefault(d => d.Start == startOfPeriod)
                 .FeeList.TryGetValue(type, out rate);

            return rate;
        }

        public List<TimeSpan> GetPeriodThresholds()
        {
            return _periodThresholds;
        }

        public List<ChargePeriodMain> GetChargeRanges()
        {
            return _periods;
        }

        public void ImportRanges()
        {
            _savedPeriods = File.ReadAllLines(_fileName);
            foreach (var line in _savedPeriods)
            {
                var period = JsonSerializer.Deserialize<ChargePeriodMain>(line);
                _periods.Add(period);
            }
        }
    }
}