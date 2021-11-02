using Contracts.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IChargeService
    {
        List<double> CalculateCharges(Dictionary<TimeSpan, double> totalDurations, VehicleTypes type);
        Dictionary<TimeSpan, double> CalculateChargePeriods(TimeRange range);

        void GetDefaultChargeValues();

        void PrintVechicleTypes();
    }
}