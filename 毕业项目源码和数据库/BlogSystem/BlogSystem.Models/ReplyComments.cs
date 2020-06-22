using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogSystem.Models
{
    /// <summary>
    /// 回复表设计
    /// </summary>
    public class ReplyComments:BaseEntity
    {
        [ForeignKey(nameof(ReplierUser))]
        public Guid ReplierId { get; set; }   //回复用户Id
        public User ReplierUser { get; set; }
        [ForeignKey(nameof(TargetToReplyUser))]
        public Guid TargetToReplyId { get; set; }   //回复目标用户Id
        public User TargetToReplyUser { get; set; }

        public int ReplyType { get; set; }      //回复类型（可能是对评论的回复、也可能是对回复的父回复）

        [ForeignKey(nameof(Comment))]
        public Guid CommentParentId { get; set; }   //评论Id
        public Comment Comment { get; set; }

        public Guid ReplyToTargetCommentId { get; set; }    //回复目标评论Id

        [Required]
        public string ReplyContent { get; set; }    //回复内容
    }
}