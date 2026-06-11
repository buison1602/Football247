using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enum
{
    public enum EnumMatchStatus
    {
        Timed,
        Scheduled,
        Live,
        InPlay,
        Paused,
        Finished,
        Postponed,
        Suspended,
        Cancelled
    }
}
