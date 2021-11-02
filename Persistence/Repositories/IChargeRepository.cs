using Contracts.Enums;
using Persistence.Models;
using System;
using System.Collections.Generic;

namespace Persistence.Repositories
{
    public interface IChargeRepository
    {
        List<TimeSpan> GetPeriodThresholds();

        double GetRates(TimeSpan startOfRange, VehicleTypes type);

        List<ChargePeriodMain> GetChargeRanges();
    }
}