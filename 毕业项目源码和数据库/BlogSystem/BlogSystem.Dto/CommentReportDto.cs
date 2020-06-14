using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Dto
{
    public class CommentReportDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }
        public string Content { get; set; }     //举报原因
        public string CommentContent { get; set; }  //被举报的评论
        public string NickName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Title { get; set; }   //所属文章标题
        public Guid ArticleId { get; set; }     //所属文章id
        public bool IsHandle { get; set; }
        public Guid ReportUserId { get; set; }      //被举报人id
    }
}
