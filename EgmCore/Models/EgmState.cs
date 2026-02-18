using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Models
{
    public enum EgmState
    {
        IDLE,
        RUNNING,
        MAINTENANCE,
        UPDATING,
        ERROR
    }
}
