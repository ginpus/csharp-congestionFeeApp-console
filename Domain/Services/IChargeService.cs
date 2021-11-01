using Contracts.Enums;
using Domain.Models;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IChargeService
    {
        Dictionary<string, double> CalculateChargePeriods(TimeRange range);

        void GetDefaultChargeValues();

        void PrintVechicleTypes();

        List<double> CalculateCharges(Dictionary<string, double> totalDurations, VehicleTypes type);
    }
}