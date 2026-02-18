using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Interfaces
{
    public interface IUpdateManager
    {
        string CurrentVersion { get; }
        string LastKnownGood { get; }
        Task InstallUpdateAsync(string packagePath);
    }
}
