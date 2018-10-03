using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;

namespace MachineCalculator.Models
{
    public class AppObj : EntityBase
    {
        public string name { get; set; }
        public bool flag { get; set; }
        public List<AppParameters> AppParameters { get; set; }
        public List<ProjectApp> ProjectApps { get; set; }


    }
}
