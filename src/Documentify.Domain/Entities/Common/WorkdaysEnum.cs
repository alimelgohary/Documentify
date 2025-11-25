using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Domain.Entities.Common
{
    [Flags]
    public enum Workdays : byte
    {
        None = 0,           // 0000_0000
        Saturday = 1 << 6,  // 0100_0000
        Sunday = 1 << 5,    // 0010_0000
        Monday = 1 << 4,    // 0001_0000
        Tuesday = 1 << 3,   // 0000_1000
        Wednesday = 1 << 2, // 0000_0100
        Thursday = 1 << 1,  // 0000_0010
        Friday = 1 << 0,    // 0000_0001
    }
}
