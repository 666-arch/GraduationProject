using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogSystem.Models
{
    /// <summary>
    /// 文章收藏（谁收藏、收藏哪篇文章）
    /// </summary>
    public class ArticleCollect:BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Article))]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}