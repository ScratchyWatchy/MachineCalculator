using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;

namespace MachineCalculator.Models
{
    public class Project : EntityBase
    {

        public string name { get; set; }
        public string description { get; set; }

        public List<ProjectApp> projectApps { get; set; }

    }
}
