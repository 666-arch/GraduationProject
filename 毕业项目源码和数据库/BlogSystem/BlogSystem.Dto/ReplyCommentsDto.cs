using System;

namespace BlogSystem.Dto
{
    public class ReplyCommentsDto
    {
        public Guid ReplierId { get; set; } //回复用户Id
        public Guid TargetToReplyId { get; set; }   //回复目标用户Id

        public Guid ReplyToTargetCommentId { get; set; }    //回复目标评论Id

        public string NickName { get; set; }    //回复人
        public string ByReplyNickName { get; set; }     //被回复人
        public string ReplyContent { get; set; }    //回复内容
        public string ImagePath { get; set; }

        public Guid Id { get; set; }    //回复表Id
        public DateTime CreateTime { get; set; }
        public Guid CommentParentId { get; set; }   //针对评论的回复
    }
}