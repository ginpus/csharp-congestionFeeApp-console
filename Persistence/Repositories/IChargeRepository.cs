using Contracts.Enums;
using Persistence.Models;
using System;
using System.Collections.Generic;

namespace Persistence.Repositories
{
    public interface IChargeRepository
    {
        List<TimeSpan> GetChargeThresholds();

        double GetRates(VehicleTypes type);

        List<ChargeRange2> GetChargeRanges();
    }
}