using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineCalculator.Models
{
    public class VM
    {

        public VM()
        {
            resourses = new List<Parameter>();
            runningApps = new List<string>();
        }
        public bool flaged { get; set; }
        public double coreLoad { get; set; }
        public double ramLoad { get; set; }
        public List<Parameter> resourses;
        public List<string> runningApps;

    }
}
