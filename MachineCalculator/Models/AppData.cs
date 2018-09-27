using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;

namespace MachineCalculator.Models
{
    public class AppData : EntityBase
    {

        public AppData(string name, int instances, List<Parameter> resourses, bool flag)
        {
            this.resourses = resourses;
            this.name = name;
            this.instances = instances;
            this.flag = flag;
            RAM = resourses[0].load;
            CPU = resourses[1].load;
        }

        public AppData(string name, int instances, List<Parameter> resourses)
        {
            this.resourses = resourses;
            this.name = name;
            this.instances = instances;
            this.flag = false;
            RAM = resourses[0].load;
            CPU = resourses[1].load;
        }

        public double RAM { get; set; }
        public double CPU { get; set; }
        [Key]
        public string name { get; set; }
        public int instances { get; set; }
        public bool flag { get; set; }
        public List<Parameter> resourses;
    }

    public class Parameter
    {
        public string name;
        public double load;

        public Parameter(string name, double load)
        {
            this.name = name;
            this.load = load;
        }
    }

}

