using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{
    /// <summary>
    /// 评论举报表
    /// </summary>
    public class CommentReport:BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }    //举报人
        public User User { get; set; }
        [ForeignKey(nameof(Comment))]
        public Guid CommentId { get; set; }     //举报的评论
        public Comment Comment { get; set; }
        public string Content { get; set; }     //举报原因
        public bool IsHandle { get; set; }      //是否已授理
    }
}
