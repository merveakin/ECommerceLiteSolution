using ECommerceLiteEntity.Enums;
using ECommerceLiteEntity.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("PassiveUsers")]
    public class PassiveUser : PersonBase
    {
        public TheIdentityRoles TargetRole { get; set; }
        public string UserId { get; set; }  //Identity Model'in ID değeri burada Foreign Key olacaktır.
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
