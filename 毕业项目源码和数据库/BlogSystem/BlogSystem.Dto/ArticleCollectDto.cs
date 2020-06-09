using System;

namespace BlogSystem.Dto
{
    public class ArticleCollectDto
    {
        public Guid ArticleId { get; set; }   //文章Id
        public string Title { get; set; } //收藏的文章
        public string NickName { get; set; }    //收藏的用户
        public DateTime DateTime { get; set; }  //收藏的时间
    }
}