using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{
    /// <summary>
    /// 评论表
    /// </summary>
    public class Comment:BaseEntity
    {
        [Required]
        [StringLength(600)]
        public string Content { get; set; }   //评论内容

        [ForeignKey(nameof(User))]  //用户外键
        public Guid UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Article))]  //文章外键
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
