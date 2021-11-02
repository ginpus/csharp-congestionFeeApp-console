using Contracts.Enums;
using Contracts.Models;
using Domain.Models;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IChargeService
    {
        List<PeriodTotalCharge> CalculateCharges(List<ChargePeriod> totalDurations, VehicleTypes type);

        List<ChargePeriod> CalculateChargePeriods(TimeRange range);

        double CalculateTotalCharge(List<PeriodTotalCharge> periodTotalCharges);

        void GetDefaultChargeValues();

        void PrintVechicleTypes();
    }
}