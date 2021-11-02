using Contracts.Enums;
using Contracts.Models;
using Domain.Models;
using Domain.Services;
using System;
using System.Collections.Generic;
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
            TimeRange timeRange = new TimeRange { };
            Dictionary<TimeSpan, double> totalDurations;
            List<double> totalCharges;

            while (true)
            {
                Console.WriteLine("Available commands:");
                Console.WriteLine("1 - Calculate Congestion Fee");
                Console.WriteLine("2 - Create new range");
                Console.WriteLine("8 - Demo Congestion Fee Calculation");
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
                        while (!DateTime.TryParseExact(end, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out endTime) || endTime < startTime)
                        {
                            Console.WriteLine("Invalid date/time, please retry");
                            if (endTime < startTime)
                            {
                                Console.WriteLine("End time cannot be before start time");
                            }
                            end = Console.ReadLine();
                        }

                        timeRange.Start = startTime;
                        timeRange.End = endTime;

                        totalDurations = _chargeService.CalculateChargePeriods(timeRange);
                        totalCharges = _chargeService.CalculateCharges(totalDurations, type);
                        foreach(var entry in totalCharges)
                        {
                            Console.WriteLine(entry);
                        }

                        Console.WriteLine("-----------------------------");
                        break;
                    case "2":
                        Console.WriteLine("Enter the name for a new range: ");
                        string alias = Console.ReadLine();
                        Console.WriteLine("Enter start time hours: ");
                        TimeSpan startHours = TimeSpan.FromHours(Convert.ToInt32(Console.ReadLine()));
                        Console.WriteLine("Enter end time hours: ");
                        TimeSpan endHours = TimeSpan.FromHours(Convert.ToInt32(Console.ReadLine()));
                        Array values = Enum.GetValues(typeof(VehicleTypes));
                        var fees = new Dictionary<VehicleTypes, double>();
                        foreach (VehicleTypes value in values)
                        {
                            Console.WriteLine($"Enter the fee for {value}: ");
                            double charge = Convert.ToDouble(Console.ReadLine());
                            fees.Add(value, charge);
                        }
                        var newRange = new ChargeRange
                        {
                            Id = Guid.NewGuid(),
                            Alias = alias,
                            Start = startHours,
                            End = endHours,
                            FeeList = fees
                        };
                        Console.WriteLine("-----------------------------");
                        break;
                    case "8":
                        Console.WriteLine("Available demos:");
                        Console.WriteLine("1 - Car: 24/04/2008 11:32 - 24/04/2008 14:42");
                        Console.WriteLine("2 - Motorbike: 24/04/2008 17:00 - 24/04/2008 22:11"); ;
                        Console.WriteLine("3 - Van: 25/04/2008 10:23 - 28/04/2008 09:02");
                        Console.WriteLine("9 - Exit Demo");
                        var chosenDemo = Console.ReadLine();
                        switch (chosenDemo)
                        {
                            case "1":
                                timeRange.Start = new DateTime(2008, 4, 24, 11, 32, 0);
                                timeRange.End = new DateTime(2008, 4, 24, 14, 42, 0);
                                var totalDurationsDemo1 = _chargeService.CalculateChargePeriods(timeRange);
                                var totalChargesDemo1 = _chargeService.CalculateCharges(totalDurationsDemo1, VehicleTypes.Car);
                                foreach (var entry in totalChargesDemo1)
                                {
                                    Console.WriteLine(entry);
                                }
                                break;
                            case "2":
                                timeRange.Start = new DateTime(2008, 4, 24, 17, 0, 0);
                                timeRange.End = new DateTime(2008, 4, 24, 22, 11, 0);
                                var totalDurationsDemo2 = _chargeService.CalculateChargePeriods(timeRange);
                                var totalChargesDemo2 = _chargeService.CalculateCharges(totalDurationsDemo2, VehicleTypes.Motorbike);
                                foreach (var entry in totalChargesDemo2)
                                {
                                    Console.WriteLine(entry);
                                }
                                break;
                            case "3":
                                timeRange.Start = new DateTime(2008, 4, 25, 10, 23, 0);
                                timeRange.End = new DateTime(2008, 4, 28, 9, 2, 0);
                                var totalDurationsDemo3 = _chargeService.CalculateChargePeriods(timeRange);
                                var totalChargesDemo3 = _chargeService.CalculateCharges(totalDurationsDemo3, VehicleTypes.Car);
                                foreach (var entry in totalChargesDemo3)
                                {
                                    Console.WriteLine(entry);
                                }
                                break;
                            case "9":
                                break;
                        }
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