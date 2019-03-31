using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba_3
{
    public class PlanObject: IComparable<PlanObject>
    {
        public DateTime TimeIn { get; set; }
        public Task Task { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime TimeOut { get; set; }
        public bool IsWaitObject { get; set; }
        public bool IsCanceled { get; set; }

        public int CompareTo(PlanObject other)
        {
            return Deadline.CompareTo(other.Deadline);
        }
    }
}
