using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;

namespace MachineCalculator.Models
{
    public class AppObj : EntityBase
    {
        [Required]
        public string name { get; set; }
        public bool flag { get; set; }
        public List<AppParameters> AppParameters { get; set; }
        public List<ProjectApp> ProjectApps { get; set; }


    }
}
