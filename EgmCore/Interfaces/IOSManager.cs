using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Interfaces
{
    public interface IOSManager
    {
        string? SetTimezone(string timezone, string requestedBy);
    }
}
