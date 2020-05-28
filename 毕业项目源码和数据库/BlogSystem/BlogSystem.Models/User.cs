using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{
    public class User:BaseEntity
    {
        [Required,StringLength(40),Column(TypeName="varchar")]  
        public string Eamil { get; set; }
        [Required, StringLength(20), Column(TypeName="varchar")]
        public string NickName { get; set; }  //昵称
        [Required, StringLength(30), Column(TypeName="varchar")]
        public string Password { get; set; }
        [Required, StringLength(300), Column(TypeName="varchar")]
        public string ImagePath { get; set; }
        public string PersonalDescription { get; set; }  //个人说明
        public int FansCount { get; set; }  //粉丝数
        public int FocusCount { get; set; } //关注数

    }
}
