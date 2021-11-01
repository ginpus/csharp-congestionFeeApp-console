using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Persistence.Repositories
{
    public class ChargeRepository : IChargeRepository
    {
        private readonly List<TimeSpan> _chargeThresholds = new List<TimeSpan>();
        private readonly List<double> _charges = new List<double>();

        private List<Range> _ranges { get; set; }

        protected string _fileName = "ranges_json.txt";

        public Range _newRange { get; set; }

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

            _chargeThresholds = chargeThresholds;
            _charges = charges;


        }

        public void ImportRanges()
        {
            _savedRanges = File.ReadAllLines(_fileName);
            foreach (var line in _savedRanges)
            {
                var entry = JsonSerializer.Deserialize<Range>(line);
                _ranges.Add(entry);
            }
        }

        public double GetRates(VehicleTypes type)
        {
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
    }
}