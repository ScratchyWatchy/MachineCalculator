using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineCalculator.Models
{

    public class AllAppsAndProject
    {
        public List<AppObj> apps { get; set; }
        public Project project { get; set; }
        public string selected{ get; set; }
        public string selectedInst { get; set; }
    }
}
