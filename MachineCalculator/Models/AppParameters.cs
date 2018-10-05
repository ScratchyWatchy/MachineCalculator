using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;

namespace MachineCalculator.Models
{
    public class AppParameters : EntityBase
    {
        public int AppId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public double load { get; set; }
        public AppObj AppObj { get; set; }

        public AppParameters(string name, double load)
        {
            this.name = name;
            this.load = load;
        }

        public AppParameters()
        {
        }
    }
}
