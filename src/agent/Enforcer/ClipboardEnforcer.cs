using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agent.Enforcer
{
    internal class ClipboardEnforcer : IPolicyEnforcer
    {
        public string name
        {
            get { return "ClipboardEnforcer"; }
        }
        public bool status
        {
            get { return status; }
            set { status = value; }
        }


        public async Task start()
        {
            status = true;

            throw new NotImplementedException();
        }

        public async Task stop()
        {
            status = false;
            throw new NotImplementedException();
        }
    {
    }
}
