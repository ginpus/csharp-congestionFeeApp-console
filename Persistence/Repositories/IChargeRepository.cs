using Contracts.Enums;
using System;
using System.Collections.Generic;

namespace Persistence.Repositories
{
    public interface IChargeRepository
    {
        List<TimeSpan> GetChargeThresholds();

        double GetRates(VehicleTypes type);
    }
}