using Contracts.Models;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class Extensions
    {
        public static ChargeRange AsDto(this ChargeRange2 model)
        {
            return new ChargeRange
            {
                Id = model.Id,
                Alias = model.Alias,
                Start = model.Start,
                End = model.End,
                FeeList = model.FeeList,
                TotalDuration = model.TotalDuration
            };
        }

    }
}
