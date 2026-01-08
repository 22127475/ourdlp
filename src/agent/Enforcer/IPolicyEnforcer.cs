using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agent.Enforcer
{

    public interface IPolicyEnforcer
    {
        string name { get; }
        bool status { get; set; }
        Task start();
        Task stop();
        
    }
}
