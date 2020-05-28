using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{

    public class Admin:BaseEntity
    {
        [Required,StringLength(20),Column(TypeName="varchar")]
        public string Account { get; set; } //账号
        [Required,StringLength(30), Column(TypeName = "varchar")]
        public string Password { get; set; }  //密码
    }
}
