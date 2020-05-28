using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{
    public class Link:BaseEntity
    {
        [Required, StringLength(30), Column(TypeName = "varchar")]
        public string Title { get; set; }   //标题
        [Required, StringLength(50), Column(TypeName = "varchar")]
        public string Url { get; set; }   //地址
        [Required, StringLength(200), Column(TypeName = "varchar")]
        public string Describe { get; set; }  //描述
    }
}
