using Domain.Models;
using Domain.Services;
using System;
using System.Threading.Tasks;

namespace CongestionFeeApp
{
    internal class CongestionFeeApp
    {
        private readonly IChargeService _chargeService;

        public CongestionFeeApp(IChargeService chargeService)
        {
            _chargeService = chargeService;
        }

        public Task Start()
        {
            var timeRange = new TimeRange
            {
                Start = new DateTime(2008, 4, 24, 11, 32, 0),
                End = new DateTime(2008, 4, 24, 14, 42, 0)
            };

            _chargeService.SplitChargableDays(timeRange);

            Console.WriteLine("The program did something");

            return Task.CompletedTask;
        }
    }
}