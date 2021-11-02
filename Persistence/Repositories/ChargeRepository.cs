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
        private readonly List<TimeSpan> _chargeThresholds = new List<TimeSpan>();
        private readonly List<double> _charges = new List<double>();

        private List<ChargeRange2> _ranges { get; set; }

        protected string _fileName = "ranges_json.txt";

        public ChargeRange2 _newRange { get; set; }

        public string[] _savedRanges { get; set; }

        public ChargeRepository()
        {
            var amStartHour = 7;
            var pmStartHour = 12;
            var pmEndHour = 19;

            var amStart = new TimeSpan(amStartHour, 0, 0);
            var pmStart = new TimeSpan(pmStartHour, 0, 0);
            var pmEnd = new TimeSpan(pmEndHour, 0, 0);

            var chargeThresholds = new List<TimeSpan> {
                amStart,
                pmStart,
                pmEnd};

            var charges = new List<double> { 2.00, 2.50 };

            _ranges = new List<ChargeRange2>();

/*            for(var i=0; i< chargeThresholds.Count -1; i++)
            {
                _newRange = new ChargeRange2
                {
                    Id = Guid.NewGuid(),
                    Alias = $"{i + 1} range",
                    Start = chargeThresholds[i],
                    End = chargeThresholds[i+1],
                    FeeList = new Dictionary<VehicleTypes, double>()
                };
                foreach(VehicleTypes value in Enum.GetValues(typeof(VehicleTypes)))
                {
                    _newRange.FeeList.Add(value, 2.22);
                }
                _ranges.Add(_newRange);
            }*/

            var amRange = new ChargeRange2
            {
                Id = Guid.NewGuid(),
                Alias = "AM rate",
                Start = new TimeSpan(7,0,0),
                End = new TimeSpan(12, 0, 0),
                FeeList = new Dictionary<VehicleTypes, double> ()
            };

            amRange.FeeList.Add(VehicleTypes.Car, 2.00);
            amRange.FeeList.Add(VehicleTypes.Motorbike, 1.00);

            _ranges.Add(amRange);

            var pmRange = new ChargeRange2
            {
                Id = Guid.NewGuid(),
                Alias = "PM rate",
                Start = new TimeSpan(12, 0, 0),
                End = new TimeSpan(19, 0, 0),
                FeeList = new Dictionary<VehicleTypes, double>()
            };
            
            pmRange.FeeList.Add(VehicleTypes.Car, 2.50);
            pmRange.FeeList.Add(VehicleTypes.Motorbike, 1.00);

            _ranges.Add(pmRange);

            _chargeThresholds = chargeThresholds;
            _charges = charges;

        }

        public void ImportRanges()
        {
            _savedRanges = File.ReadAllLines(_fileName);
            foreach (var line in _savedRanges)
            {
                var entry = JsonSerializer.Deserialize<ChargeRange2>(line);
                _ranges.Add(entry);
            }
        }

        public double GetRates(VehicleTypes type)
        {
/*            var ex = new TimeSpan(0, 0, 0);
            _ranges
                .Where(d => d.Start.TotalMinutes <= ex.TotalMinutes &&
            d.End.TotalMinutes > ex.TotalMinutes)
                .Where(b => b.FeeList.Keys.SingleOrDefault(type));
            
            foreach(var value in _ranges)
            {

            }*/
            return _charges[0];
        }

/*        public void InsertNote()
        {
            Console.Write("Range title: ");
            string title = Console.ReadLine();
            Console.Write("Range start: ");
            string name = Console.ReadLine();
            _newRange = new Range { };
            _notepad.Add(_newNote); // inserting into object list
            string newJsonNote = JsonSerializer.Serialize<Range>(_newNote); //inserting into JSON file
            File.AppendAllLines(_fileNameJ, new[] { newJsonNote });
            Console.Write($"-> New note inserted: \n{_newNote}");
            //Console.Write($"-> New note inserted: \n{_newNote.ToString()}\n{newJsonNote}");
        }*/

        public List<TimeSpan> GetChargeThresholds()
        {
            return _chargeThresholds;
        }

        public List<double> GetCharges()
        {
            return _charges;
        }

        public List<ChargeRange2> GetChargeRanges()
        {
            return _ranges;
        }
    }
}