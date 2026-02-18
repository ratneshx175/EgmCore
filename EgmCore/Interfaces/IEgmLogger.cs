using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Interfaces
{
    public interface IEgmLogger
    {
        void Log(string message);
        void SetTimeZone(string timeZoneId);
    }
}
