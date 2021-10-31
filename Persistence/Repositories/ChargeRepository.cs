using System;
using System.Collections.Generic;

namespace Persistence.Repositories
{
    public class ChargeRepository : IChargeRepository
    {
        private readonly List<TimeSpan> _chargeThresholds = new List<TimeSpan>();
        private readonly List<double> _charges = new List<double>();

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