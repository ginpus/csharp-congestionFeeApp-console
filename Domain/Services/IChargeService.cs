using Contracts.Enums;
using Contracts.Models;
using Domain.Models;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IChargeService
    {
        //List<double> CalculateCharges(Dictionary<TimeSpan, double> totalDurations, VehicleTypes type);
        List<PeriodTotalCharge> CalculateCharges(List<ChargeRange> totalDurations, VehicleTypes type);
        
        //Dictionary<TimeSpan, double> CalculateChargePeriods(TimeRange range);

        List<ChargeRange> CalculateChargePeriods(TimeRange range);

        double CalculateTotalCharge(List<PeriodTotalCharge> periodTotalCharges);

        void GetDefaultChargeValues();

        void PrintVechicleTypes();
    }
}