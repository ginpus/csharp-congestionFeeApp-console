using System;
using System.Collections.Generic;

namespace Persistence.Repositories
{
    public interface IChargeRepository
    {
        List<TimeSpan> GetChargeThresholds();
    }
}