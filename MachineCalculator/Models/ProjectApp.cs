using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;

namespace MachineCalculator.Models
{
    public class ProjectApp :EntityBase
    {
        public int projectId { get; set; }
        public Project Project { get; set; }
        public int appId { get; set; }
        public AppObj AppObj { get; set; }
        [Required]
        public int instances { get; set; }
    }
}
