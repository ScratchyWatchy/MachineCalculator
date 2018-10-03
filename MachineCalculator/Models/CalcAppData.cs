using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserDBWebRest.Business;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineCalculator.Models
{
    public class CalcAppData : EntityBase
    {

        public CalcAppData(string name, int instances, List<AppParameters> resourses, bool flag)
        {
            this.resourses = resourses;
            this.name = name;
            this.instances = instances;
            this.flag = flag;
            RAM = resourses[0].load;
            CPU = resourses[1].load;
        }

        public CalcAppData(string name, int instances, List<AppParameters> resourses)
        {
            this.resourses = resourses;
            this.name = name;
            this.instances = instances;
            this.flag = false;
            RAM = resourses[0].load;
            CPU = resourses[1].load;
        }

        public CalcAppData()
        {
        }

        public double RAM { get; set; }
        public double CPU { get; set; }
        public string name { get; set; }
        public int instances { get; set; }
        public bool flag { get; set; }
        public List<AppParameters> resourses { get; set; }
    }
}

