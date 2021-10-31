using Domain.Models;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IChargeService
    {
        List<TimeSplit> SplitChargableDays(TimeRange range);
    }
}