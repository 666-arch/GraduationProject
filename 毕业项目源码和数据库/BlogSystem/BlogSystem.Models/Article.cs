using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{
    public class Article:BaseEntity
    {
        [Required]
        public string Title { get; set; }

        [Required,Column(TypeName = "ntext")]  //最多2G
        public string Content { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }  //用户外键
        public User User { get; set; }
        public int GoodCounnt { get; set; }
        public int BadCount { get; set; }
        public bool State { get; set; }  //发布状态

        public bool IsClosingComments { get; set; }     //是否关闭评论功能

        public int IsStick { get; set; }      //置顶排序

    }
}
