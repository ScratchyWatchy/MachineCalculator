using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UserDBWebRest.Business
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}