using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Dto
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public string NickName { get; set; } //昵称
        public int GoodCount { get; set; }
        public int BadCount { get; set; }
        public string ImagePath { get; set; }
        public bool State { get; set; } //发布状态
        public string[] CategoryNames { get; set; }
        public Guid[] CategoryIds { get; set; }
        public bool IsClosingComments { get; set; }
        public int IsStick{ get; set; }
    }
}
