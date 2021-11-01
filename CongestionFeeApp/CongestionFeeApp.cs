using Contracts.Enums;
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
            VehicleTypes type;
            string start;
            string end;
            DateTime startTime;
            DateTime endTime;

            //can be deleted. for printing only
            _chargeService.GetDefaultChargeValues();

            while (true)
            {
                Console.WriteLine("Available commands:");
                Console.WriteLine("1 - Calculate Congestion Fee");;
                Console.WriteLine("9 - Exit");

                var chosenCommand = Console.ReadLine();
                switch (chosenCommand)
                {
                    case "1":
                        Console.WriteLine("Enter vechicle type: ");
                        _chargeService.PrintVechicleTypes();
                        type = (VehicleTypes)Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Enter start time in `dd/MM/yyyy HH:mm` format (no ticks): ");
                        start = Console.ReadLine();
                        while (!DateTime.TryParseExact(start, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out startTime))
                        {
                            Console.WriteLine("Invalid date/time, please retry");
                            start = Console.ReadLine();
                        }

                        Console.WriteLine("Enter end time in `dd/MM/yyyy HH:mm` format (no ticks): ");
                        end = Console.ReadLine();
                        while (!DateTime.TryParseExact(end, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out endTime))
                        {
                            Console.WriteLine("Invalid date/time, please retry");
                            end = Console.ReadLine();
                        }

                        var timeRange = new TimeRange
                        {
                            Start = startTime,
                            End = endTime
                        };

                        _chargeService.SplitChargableDays(timeRange);
                        Console.WriteLine("-----------------------------");
                        break;

                    case "9":
                        Console.WriteLine("The program ended");
                        return Task.CompletedTask;
                }
            }
        }
    }
}